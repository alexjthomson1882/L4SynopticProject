using MusicPlayer.Data;
using MusicPlayer.Media;

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MusicPlayer {

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application {

        #region constant

        private static readonly ConcurrentQueue<DispatchedHandler> RunAsyncQueue = new ConcurrentQueue<DispatchedHandler>();

        #endregion

        #region variable

        private readonly MediaManager mediaManager;

        private readonly AudioPlayer audioPlayer;

        #endregion

        #region property

        public AudioPlayer AudioPlayer => audioPlayer;

        public MediaManager MediaManager => mediaManager;

        public static CoreDispatcher Dispatcher { get; private set; }

        #endregion

        #region constructor

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() {
            // initialise application and callbacks:
            InitializeComponent();
            Suspending += OnSuspending;
            // initialise database:
            DataAccess.InitialiseDatabase();
            // create media manager:
            mediaManager = new MediaManager();
            // create audio player:
            audioPlayer = new AudioPlayer();
        }

        #endregion

        #region logic

        #region OnLaunched

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected sealed override void OnLaunched(LaunchActivatedEventArgs e) {

            // update dispatcher:
            CoreWindow window = CoreWindow.GetForCurrentThread();
            window.Activate();
            Dispatcher = window.Dispatcher;

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null) {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated) {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false) {
                if (rootFrame.Content == null) {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        #endregion

        #region OnNavigationFailed

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e) {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        #endregion

        #region OnSuspending

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e) {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        #endregion

        #region QueueRunAsync

        public static async void QueueRunAsync(DispatchedHandler dispatchedHandler) {
            if (dispatchedHandler == null) throw new ArgumentNullException(nameof(dispatchedHandler));
            RunAsyncQueue.Enqueue(dispatchedHandler);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                while (RunAsyncQueue.TryDequeue(out DispatchedHandler handler)) {
                    handler.Invoke();
                }
            });
        }

        #endregion

        #endregion

    }

}
