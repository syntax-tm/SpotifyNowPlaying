using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using log4net;
using SpotifyNowPlaying.Common;

namespace SpotifyNowPlaying.ViewModels
{
    public class BrowserViewModel
    {
        private const int ERROR_ACCESS_DENIED = 5;

        protected readonly ILog log = LogManager.GetLogger(typeof(BrowserViewModel));

        protected virtual ICurrentWindowService CurrentWindow { get { return null; } }

        public Uri ResponseUri { get; protected set; }
        public string ResponseContents { get; protected set; }
        public virtual Uri SourceUri { get; set; }
        public string CallbackUrl { get; }
        public bool? DialogResult { get; protected set; }

        protected BrowserViewModel()
        {

        }

        protected BrowserViewModel(Uri sourceUri, string callbackUrl)
        {
            SourceUri = sourceUri;
            CallbackUrl = callbackUrl;

            Task.Run(StartListener).ConfigureAwait(false);
        }

        public static BrowserViewModel Create()
        {
            return ViewModelSource.Create(() => new BrowserViewModel());
        }

        public static BrowserViewModel Create(Uri sourceUri, string callbackUrl)
        {
            return ViewModelSource.Create(() => new BrowserViewModel(sourceUri, callbackUrl));
        }

        protected void StartListener()
        {
            var retry = false;

            do
            {
                try
                {
                    using var listener = new HttpListener();
                    listener.Prefixes.Add(CallbackUrl);

                    listener.Start();

                    var context = listener.GetContext();
                    var request = context.Request;

                    using var receiveStream = request.InputStream;
                    using var readStream = new StreamReader(receiveStream, Encoding.UTF8);

                    ResponseContents = readStream.ReadToEnd();
                    ResponseUri = request.Url;

                    log.Debug($"Callback received at '{ResponseUri}'");

                    DialogResult = true;

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        CurrentWindow.Close();
                    }));
                }
                catch (HttpListenerException he) when (he.ErrorCode == ERROR_ACCESS_DENIED)
                {
                    log.Error($"An {nameof(HttpListenerException)} occurred ({he.ErrorCode}). {he.Message}", he);

                    NetAclChecker.AddAddress(CallbackUrl);

                    retry = true;
                }
                catch (HttpListenerException he)
                {
                    throw new Exception($"An {nameof(HttpListenerException)} occurred ({he.ErrorCode}). {he.Message}", he);
                }
            }
            while (retry);
        }
    }
}
