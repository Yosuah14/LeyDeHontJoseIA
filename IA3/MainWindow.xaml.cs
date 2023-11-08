using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace IA3
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Partido> Partidos { get; set; }
        private List<Partido> listaPartidos = new List<Partido>();

        public MainWindow()
        {
            InitializeComponent();
            Partidos = new ObservableCollection<Partido>();
            partidosDataGrid.ItemsSource = Partidos;

            txtPopulation.Text = "6921267";
            txtPopulation.IsEnabled = false;
        }

        // Guardar y habilitar siguiente pestaña
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            txtPopulation.Text = "6921267";
            if (int.TryParse(txtAbstentionVotes.Text, out int abstentionVotes) && abstentionVotes <= 6921267 && abstentionVotes > 0)
            {
                int nullVotes = (6921267 - abstentionVotes) / 20;
                txtNullVotes.Text = nullVotes.ToString();

                TabControl tabControl = (TabControl)this.FindName("tabControl");
                tabControl.SelectedIndex += 1;

                part.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Por favor, introduce un conteo de abstenciones válido (debe ser un número entre 1 y 6921267).");
            }
        }

        // Insertar nuevo partido
        private void InsertarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtNombre.Text) && !string.IsNullOrWhiteSpace(txtAcronimo.Text) && !string.IsNullOrWhiteSpace(txtPresidente.Text))
                {
                    if (Partidos.Count < 10)
                    {
                        Partido nuevoPartido = new Partido(txtNombre.Text, txtAcronimo.Text, txtPresidente.Text, Partidos.Count + 1, 6921267, int.Parse(txtAbstentionVotes.Text));

                        Partidos.Add(nuevoPartido);
                        listaPartidos.Add(nuevoPartido);

                        if (Partidos.Count == 10)
                        {
                            data.IsEnabled = false;
                            part.IsEnabled = false;
                            simulate.IsEnabled = true;
                            simulate.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Se ha alcanzado el límite de 10 partidos.");
                    }
                }
                else
                {
                    MessageBox.Show("Completa todos los campos para insertar un nuevo partido.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Borrar partido
        private void BorrarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (partidosDataGrid.SelectedItem != null)
                {
                    Partidos.Remove((Partido)partidosDataGrid.SelectedItem);
                    borrarButton.Visibility = Partidos.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Actualizar los votos nulos basados en las abstenciones
        private void txtAbstentionVotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(txtAbstentionVotes.Text, out int abstentionVotes) && abstentionVotes <= 6921267 && abstentionVotes > 0)
            {
                int nullVotes = (6921267 - abstentionVotes) / 20;
                txtNullVotes.Text = nullVotes.ToString();
            }
            else
            {
                // Mensaje de error u otras acciones si el valor no es válido
            }
        }

        // Acciones al seleccionar un partido
        private void PartidosDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                borrarButton.Visibility = partidosDataGrid.SelectedItem != null ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Simular la asignación de escaños
        private bool simulateb=false;
        private void SimulateButton_Click(object sender, RoutedEventArgs e)
        {
            if (simulateb) {
                ResetData();
            }

            simulate.Focus();
            AssignSeatsToParties(listaPartidos);

            var resultados = listaPartidos
                .Where(p => p.Escanos > 0)
                .Select(p => new Resultado
                {
                    Nombre = p.Nombre,
                    Votos = p.VotosValidos,
                    Escaños = p.Escanos
                })
                .ToList();

            resultDataGrid.ItemsSource = resultados;
            simulateb = true;
  
        }

        // Método para asignar escaños a los partidos que superan el 3% de los votos
        private void AssignSeatsToParties(List<Partido> partidos)
        {
            int totalVotosValidos = partidos.Sum(p => p.VotosValidos);
            int porcentajeMinimo = (int)(0.03 * totalVotosValidos);

            var partidosElegibles = partidos.Where(p => p.VotosValidos >= porcentajeMinimo).ToList();
            partidosElegibles = partidosElegibles.OrderByDescending(p => p.VotosValidos).ToList();

            int escañosAsignados = 37;

            while (escañosAsignados > 0)
            {
                int maxCociente = 0;
                Partido partidoGanador = null;

                foreach (var partido in partidosElegibles)
                {
                    int cociente = partido.VotosValidos / (partido.Escanos + 1);

                    if (cociente > maxCociente)
                    {
                        maxCociente = cociente;
                        partidoGanador = partido;
                    }
                }

                if (partidoGanador != null)
                {
                    partidoGanador.Escanos++;
                    escañosAsignados--;
                }
                else
                {
                    break;
                }
            }
        }

            private void Back_Button(object sender, RoutedEventArgs e)
            {
                ResetData();
            }
            private void ResetData()
            {
                // Limpia los datos
                Partidos.Clear();
                listaPartidos.Clear();
                resultDataGrid.ItemsSource = null;
                txtAbstentionVotes.Text = "";
                txtNullVotes.Text = "";
                borrarButton.Visibility = Visibility.Collapsed;

                // Establece el foco en la pestaña de datos
                TabControl tabControl = (TabControl)this.FindName("tabControl");
                tabControl.SelectedIndex = 0;

                // Habilita o deshabilita los botones según sea necesario
                simulate.IsEnabled = false;
                data.IsEnabled = true;
                part.IsEnabled = false;
                part.Focus();
            }
        }
    }
      
