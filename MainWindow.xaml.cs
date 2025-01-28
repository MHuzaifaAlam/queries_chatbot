using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Python.Runtime;
using System.Windows.Media;


namespace quries_chatbot
{
	public partial class MainWindow : Window
	{
		// Define the structure of each chat message
		public class ChatMessage
		{
			public string Message { get; set; } = string.Empty;
			public string Foreground { get; set; } = "#333"; // Default text color
			public string Background { get; set; } = "#E8E8E8"; // Default message background color
			public bool IsUserMessage { get; set; } // Determines if the message is from the user
		}
		public ObservableCollection<ChatMessage> ChatHistory { get; set; }

		public MainWindow()
		{
			InitializeComponent(); // This initializes all XAML components
			ChatHistory = new ObservableCollection<ChatMessage>();
			ChatHistoryListBox.ItemsSource = ChatHistory;
		}

        private async void SendQuery_Click(object sender, RoutedEventArgs e)
        {
            string query = UserQuery.Text; // Get the user query
            string apiUrl = "http://1234abcd.ngrok.io/predict"; // Replace with your FastAPI URL

            // Create an HTTP client
            using (HttpClient client = new HttpClient())
            {
                // Prepare the request payload
                var payload = new { query = query };
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                try
                {
                    // Send POST request
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response
                        string responseString = await response.Content.ReadAsStringAsync();
                        var responseObject = JsonSerializer.Deserialize<ChatbotResponse>(responseString);

                        // Display the response
                        ChatbotResponse.Text = responseObject.intent; // Adjust based on your API response
                    }
                    else
                    {
                        ChatbotResponse.Text = "Error: " + response.StatusCode;
                    }
                }
                catch (Exception ex)
                {
                    ChatbotResponse.Text = "Error: " + ex.Message;
                }
            }
        }

        // Response class for parsing API response
        public class ChatbotResponse
        {
            public string intent { get; set; }
        }


        private string GetBotResponse(string query)
		{
			string response = "I'm sorry, I didn't understand that.";
			using (Py.GIL())
			{
				try
				{
					dynamic sys = Py.Import("sys");

					// Append the directory of the executable
					string scriptPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
					Console.WriteLine($"Appending Path: {scriptPath}");
					sys.path.append(scriptPath);

					// Import the script module
					dynamic chatbotModule = Py.Import("script");
					response = chatbotModule.get_response(query).ToString();
				}
				catch (PythonException ex)
				{
					response = $"Error: {ex.Message}";
				}
			}
			return response;
		}

		private void QueryInputTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			if (QueryInputTextBox.Text == "Type your query here...")
			{
				QueryInputTextBox.Text = "";
				QueryInputTextBox.Foreground = new SolidColorBrush(Colors.Black); // Set to black when typing
			}
		}

		private void QueryInputTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(QueryInputTextBox.Text))
			{
				QueryInputTextBox.Text = "Type your query here...";
				QueryInputTextBox.Foreground = new SolidColorBrush(Colors.Gray); // Set to gray for placeholder
			}
		}


	}
}