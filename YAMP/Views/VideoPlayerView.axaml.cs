using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LibVLCSharp.Shared;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using YAMP.Utils;
using YAMP.ViewModels;
using LibVLCSharp.Avalonia;
using Avalonia.Threading;

namespace YAMP.Views
{
    public partial class VideoPlayerView : Window
    {
        public VideoPlayerViewModel viewModel = new VideoPlayerViewModel();
        static VideoPlayerView? _this;
        //public static ControlsPanelView? ControlsView = new ControlsPanelView();
        public static ControlsPanelView? ControlsView;

        public Panel mpContainer;
        
        public VideoView player;
        

        public string? videoUrl { get; set; }
        public string? coverUrl { get; set; }
        public string? videoDuration { get; set; }
        public string? videoTitle { get; set; }
        public int videoWidth { get; set; }
        public int videoHeight { get; set; }

        public string videoAspectRatio { get; set; }

        

        //private LibVLCSharp.Avalonia.VideoView mpContainer;

        
        //private Panel flyPanelContainer;
        
        public VideoPlayerView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif            
            _this = this;            

            DataContext = viewModel;

            mpContainer = this.Get<Panel>("MPContainer");                        
                       

            Opened += VideoPlayerView_Opened;
            Closing += VideoPlayerView_Closing;
        }

        public static VideoPlayerView GetInstance()
        {
            return _this;
        }
        
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }        

        public void ResizePlayerWindow()
        {
            // Adapt Video Player window to video dimensions proportionally
            var pixelDensity = Screens.Primary.PixelDensity;

            // Screen working area dimensions
            var rect = Screens.Primary.WorkingArea;
            var containerW = rect.Width;
            var containerH = rect.Height;            

            double W = 0;
            double H = 0;

            // Set Player window dimensions
            Utilities.SetWindowBounds(pixelDensity, containerW, containerH, this.Width, this.Height, out W, out H, false);

            this.Width = W;
            this.Height = H;

            //double W = this.Width;
            //double H = this.Height;            

            if (videoWidth > 0 && videoHeight > 0)
            {
                double aspectRatio = (videoWidth * 1.0) / (videoHeight * 1.0);
                W = W * aspectRatio;

                //double dxFactor = videoWidth * 100.0 / W;
                //double dyFactor = videoHeight * 100.0 / H;
                

                //double dxFactor = (double)videoWidth / W;
                //double dyFactor = (double)videoHeight / H;
                //W *= dxFactor;
                //H *= dyFactor;
                
            }

            this.Width = W;
            this.Height = H;

        }

        private void VideoPlayerView_Opened(object? sender, EventArgs e)
        {
            ControlsView = new ControlsPanelView();            

            var ctrls = ControlsPanelView.GetInstance();
            this.Topmost = false;

            ControlsView.timeSlider.Maximum = 100.0;
            ControlsView.timeSlider.Minimum = 0.0;
            ControlsView.timeSlider.Value = 0.0;
            ControlsView.volumeSlider.Value = 50.0;

            ControlsView.viewModel.XVolume = 50.0;
            ControlsView.viewModel.XTime = 0.0;            

            if (null != videoUrl)
            {
                ControlsView.viewModel.VideoDurationString = videoDuration;
                ControlsView.viewModel.VideoTitle = videoTitle;                            

                viewModel.StartPlay(videoUrl, coverUrl);
            }

            ControlsView.Height = 100;
            ControlsView.Position = this.Position;
            ControlsView.Width = this.Width;

            // Vertical videos and videos with width < height
            if (this.Width < this.Height)
            {
                ControlsView._full.IsVisible = false;

            }

            ControlsView.Show();            

        }

        private void VideoPlayerView_Closing(object? sender, EventArgs e)
        {
            viewModel.PlayerIsExiting();
        }

        public async void CloseDialog_Click()
        {
            await Dispatcher.UIThread.InvokeAsync(() => 
            {
                ControlsView.Hide();
                ControlsView.Close();

                Close();
            }).ConfigureAwait(false);
            
        }
        
        /// <summary>
        /// Get video dimensions from MediaPlayer tracks or from params (yt-dlp ephemeral)
        /// </summary>
        /// <param name="videoW"></param>
        /// <param name="videoH"></param>
        private void GetVideoDimensions(ref uint videoW, ref uint videoH)
        {
            var mediaTrack = viewModel.MediaPlayer.Media?.Tracks?.FirstOrDefault(x => x.TrackType == TrackType.Video);
            if (mediaTrack != null)
            {
                var videoSwapped = mediaTrack.Value.Data.Video.Orientation == VideoOrientation.LeftBottom ||
                                            mediaTrack.Value.Data.Video.Orientation == VideoOrientation.RightTop;

                videoW = mediaTrack.Value.Data.Video.Width;
                videoH = mediaTrack.Value.Data.Video.Height;


                if (videoSwapped)
                {
                    var swap = videoW;
                    videoW = videoH;
                    videoH = swap;
                }

                if (mediaTrack.Value.Data.Video.SarNum != mediaTrack.Value.Data.Video.SarDen)
                    videoW = videoW * mediaTrack.Value.Data.Video.SarNum / mediaTrack.Value.Data.Video.SarDen;

                //Debug.WriteLine($"Video Size(from MediaTrack)={videoW}:{videoH}");
            }
            
            if (mediaTrack == null || videoW == 0 || videoH == 0)
            {
                // Size() fails with some videos. Take the dimensions from passed params (videoWidth/videoHeight)
                //viewModel.MediaPlayer.Size(0, ref videoW, ref videoH);
                videoW = (uint)videoWidth;
                videoH = (uint)videoHeight;
                //Debug.WriteLine($"Video Size={videoW}:{videoH}");
            }
        }

        public void SetMediaPlayerScale()
        {
            uint videoW = 0, videoH = 0;
            double containerW, containerH;
            var pixelDensity = Screens.Primary.PixelDensity;

            GetVideoDimensions(ref videoW, ref videoH);
            
            var rect = mpContainer.Bounds;
            containerW = rect.Width;
            containerH = rect.Height;
            

            //Debug.WriteLine($"Container Size={containerW}:{containerH} Pixel Density={pixelDensity}");            

            /*
            // Doesn't fill the container. Keep ratio of width/height
            var ar = (float)(videoW / videoH);
            var dar = (float)(containerW * pixelDensity / containerH * pixelDensity);
                                      
            float scale;
            if (dar >= ar)
                scale = (float)(containerW * pixelDensity / videoW); // horizontal
            else
                scale = (float)(containerH * pixelDensity / videoH); // vertical
            
            */

            
            // Fill the container. Video is stretched            
            float xscale = (float)Math.Round(containerW * pixelDensity / videoW, 8);
            float yscale = (float)Math.Round(containerH * pixelDensity / videoH, 8);

            float scale = (xscale < yscale) ? xscale : yscale;
            

            viewModel.MediaPlayer.Scale = scale;
            

            //viewModel.MediaPlayer.Scale = 0;

            // Aspect Ratio            
            var aspectRatio = String.Format("{0}:{1}", Convert.ToInt32(containerW), Convert.ToInt32(containerH));
            //var aspectRatio = String.Format("{0}:{1}", videoW, videoH);

            viewModel.MediaPlayer.AspectRatio = aspectRatio;
            

            //viewModel.MediaPlayer.AspectRatio = videoAspectRatio;

            //Debug.WriteLine($"Scale={scale}  AspectRatio={aspectRatio}");
        }

        public void FullScreen_Click()
        {            
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

            SetMediaPlayerScale();
            ControlsView.Position = this.Position;
            ControlsView.Width = this.Width;
        }
    }
}
