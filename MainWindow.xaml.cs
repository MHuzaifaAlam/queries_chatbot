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

		private async void SendButton_Click(object sender, RoutedEventArgs e)
		{
			string userQuery = QueryInputTextBox.Text.Trim();
			if (string.IsNullOrEmpty(userQuery)) return;

			// Add user's query to chat
			ChatHistory.Add(new ChatMessage
			{
				Message = userQuery,
				Foreground = "#000000",
				Background = "#E0E0E0",
				IsUserMessage = true
			});
			QueryInputTextBox.Clear();

			// Get chatbot response
			string botResponse = await Task.Run(() => GetBotResponse(userQuery));

			// Add bot response to chat
			ChatHistory.Add(new ChatMessage
			{
				Message = botResponse,
				Foreground = "#FFFFFF",
				Background = "#10A37F",
				IsUserMessage = false
			});
			ChatHistoryListBox.ScrollIntoView(ChatHistoryListBox.Items[ChatHistoryListBox.Items.Count - 1]);
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