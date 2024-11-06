namespace Camera_App
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private int horizontalAngle = 90;  // Starts at 90°
        private int verticalAngle = 90;    // Starts at 90°


        public MainPage()
        {
            InitializeComponent();
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {

        }

        // Update the display for horizontal and vertical angles
        private void UpdateAngleLabels()
        {
            horizontalAngleLabel.Text = $"Horizontal Angle: {horizontalAngle}°";
            verticalAngleLabel.Text = $"Vertical Angle: {verticalAngle}°";
        }

        // When the "Up" button is clicked, increase the vertical angle
        private void OnUpClicked(object sender, EventArgs e)
        {
            if (verticalAngle < 180)
            {
                verticalAngle += 10;
                UpdateAngleLabels();
            }
        }

        // When the "Down" button is clicked, decrease the vertical angle
        private void OnDownClicked(object sender, EventArgs e)
        {
            if (verticalAngle > 0)
            {
                verticalAngle -= 10;
                UpdateAngleLabels();
            }
        }

        // When the "Left" button is clicked, decrease the horizontal angle
        private void OnLeftClicked(object sender, EventArgs e)
        {
            if (horizontalAngle > 0)
            {
                horizontalAngle -= 10;
                UpdateAngleLabels();
            }
        }

        // When the "Right" button is clicked, increase the horizontal angle
        private void OnRightClicked(object sender, EventArgs e)
        {
            if (horizontalAngle < 180)
            {
                horizontalAngle += 10;
                UpdateAngleLabels();
            }
        }
    }

}
