namespace ProyectoReservaCanchasMAUI.Views
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }


        private void CounterBtn_Clicked(object sender, EventArgs e)
        {
            count++;
            if (count == 1)
            {
                CounterBtn.Text = $"Has hecho clic {count} vez";
            }
            else
            {
                CounterBtn.Text = $"Has hecho clic {count} veces";
            }
            SemanticScreenReader.Announce(CounterBtn.Text);

        }
    }

}
