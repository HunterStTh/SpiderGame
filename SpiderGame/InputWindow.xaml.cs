// Гаврилов Д
/* класс окна для ввода имени нового паука.
Функционал:
Проверка на пустое имя.
Проверка уникальности имени через ExistingNames.
Обработка ввода с клавиатуры (Enter для подтверждения).
Возврат результата через DialogResult.*/
using System.Windows;
using System.Windows.Input;

namespace SpiderGame
{
    public partial class InputWindow : Window
    {
        // Свойство для хранения введенного имени паука 
        public string SpiderName { get; private set; }
        public List<string> ExistingNames { get; set; } = new List<string>();

        // Конструктор окна
        public InputWindow(List<string> existingNames)
        {
            InitializeComponent(); // Инициализация компонентов
            ExistingNames = existingNames;
        }
        // Обработчик нажатия кнопки OK
        // sender — это кнопка "OK"
        // RoutedEventArgs e Содержит информацию о событии
        /* Содержит информацию о событии, например:
        Какое событие произошло.
        Какой элемент вызвал событие.
        */
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SpiderName = NameTextBox.Text.Trim(); // Получаем и очищаем введенное имя
            if (string.IsNullOrWhiteSpace(SpiderName)) // Проверка на пустое имя
            {
                MessageBox.Show("Имя не может быть пустым!"); // Показываем ошибку
                return;
            }
            // Проверка на уникальность имени (используем вызов в MainWindow)
            var mainWindow = Application.Current.Windows
                .OfType<MainWindow>()
                .FirstOrDefault();

            if (ExistingNames.Contains(SpiderName.ToLower()))
            {
                MessageBox.Show("Паук с таким именем уже существует!");
                return;
            }
            DialogResult = true; // Устанавливаем результат диалога как "Успешно"
            Close(); // Закрываем окно
        }

        // Обработчик нажатия клавиши в текстовом поле
        // sender: TextBox (поле ввода имени)
        // e.Key: Проверяем, какая клавиша нажата (например, Enter)
        private void NameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Вызываем метод кнопки "OK"
                OkButton_Click(sender, e);
                // Запрещаем дальнейшую обработку клавиши
                e.Handled = true;
            }
        }

        // Обработчик нажатия кнопки "Отмена"
        // sender — это кнопка "Отмена"
        // RoutedEventArgs e Содержит информацию о событии
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Устанавливаем результат диалога как "Отмена"
            Close(); // Закрываем окно
        }
    }
}