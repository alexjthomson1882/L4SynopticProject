using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MusicPlayer {

    /// <summary>
    /// Main window content. The <see cref="MainPage"/> defines the basic layout for the entire application.
    /// </summary>
    public sealed partial class MainPage : Page {

        #region constant

        #endregion

        #region constructor

        public MainPage() {
            InitializeComponent();
        }

        #endregion

        #region logic
        
        #region PageContent

        private void PageContent_Loaded(object sender, RoutedEventArgs e) {
            NavigateTo<HomePage>();
        }

        #endregion

        #region NavigateTo

        /// <summary>
        /// Navigates to a <see cref="Page"/> of type <typeparamref name="T"/>.
        /// </summary>
        public void NavigateTo<T>() where T : Page => ContentFrame.Navigate(typeof(T));

        #endregion

        #region HomeButton

        public void HomeButton_Click(object sender, RoutedEventArgs e) {
            NavigateTo<HomePage>();
        }

        #endregion

        #region SearchButton

        public void SearchButton_Click(object sender, RoutedEventArgs e) {
            NavigateTo<SearchPage>();
        }

        #endregion

        #region LibraryButton

        public void LibraryButton_Click(object sender, RoutedEventArgs e) {
            NavigateTo<LibraryPage>();
        }

        #endregion

        #region CreatePlaylist

        public void CreatePlaylistButton_Click(object sender, RoutedEventArgs e) {
            NavigateTo<CreatePlaylistPage>();
        }

        #endregion

        #endregion

    }
}
