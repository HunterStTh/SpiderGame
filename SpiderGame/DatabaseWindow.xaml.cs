// Гаврилов Д
using System.Windows;
using System.Windows.Controls;

namespace SpiderGame
{
    public partial class DatabaseWindow : Window
    {
        // Событие для уведомления главного окна (MainWindow) об изменениях в базе.
        public event Action SpidersUpdated;

        // Контекст базы данных для выполнения операций с БД.
        private SpiderDbContext _dbContext;

        // Конструктор окна базы данных.
        public DatabaseWindow()
        {
            InitializeComponent();
            // Создание контекста базы данных.
            _dbContext = new SpiderDbContext();
            // Загрузка данных при инициализации окна.
            LoadData();
        }

        // Загрузка данных из таблицы Spiders в DataGrid.
        private void LoadData()
        {
            // Привязка списка пауков к DataGrid.
            SpidersDataGrid.ItemsSource = _dbContext.Spiders.ToList();
        }

        // Обработчик кнопки "Добавить паука".
        private void AddSpider_Click(object sender, RoutedEventArgs e)
        {
            // Получение списка существующих имен для проверки уникальности.
            var existingNames = _dbContext.Spiders.Select(s => s.Name.ToLower()).ToList();

            // Создание окна ввода имени.
            var inputWindow = new InputWindow(existingNames);

            // Если пользователь подтвердил ввод.
            if (inputWindow.ShowDialog() == true)
            {
                string newName = inputWindow.SpiderName;

                // Проверка уникальности имени.
                if (!existingNames.Contains(newName.ToLower()))
                {
                    // Создание объекта паука.
                    var spider = new Spider(inputWindow.SpiderName, 100, 50, DateTime.Now);

                    // Добавление паука в БД.
                    _dbContext.Spiders.Add(spider);
                    _dbContext.SaveChanges();

                    // Обновление таблицы и оповещение подписчиков.
                    LoadData();
                    SpidersUpdated?.Invoke();
                }
                else
                {
                    MessageBox.Show("Паук с таким именем уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // Обработчик кнопки "Удалить паука".
        private void DeleteSpider_Click(object sender, RoutedEventArgs e)
        {
            // Если выбран паук в DataGrid.
            if (SpidersDataGrid.SelectedItem is Spider selectedSpider)
            {
                // Удаление паука из БД.
                _dbContext.Spiders.Remove(selectedSpider);
                _dbContext.SaveChanges();

                // Обновление таблицы и оповещение подписчиков.
                LoadData();
                SpidersUpdated?.Invoke();
            }
        }

        // Обработчик текстбокса "Поиск паука".
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Возвращает копию этой строки, переведенную в нижний регистр.
            var searchText = SearchTextBox.Text.ToLower();

            var filtered = _dbContext.Spiders
                // Переключает выполнение запроса с серверной части (база данных) на клиентскую (память приложения).
                /* До вызова .AsEnumerable() запрос представлен как IQueryable<T>, который генерирует SQL и выполняется на стороне БД.
                 * После .AsEnumerable() запрос становится IEnumerable<T>, и все последующие операции (например, Where) выполняются в памяти приложения.*/
                .AsEnumerable()
                //Фильтрует данные по условию.
                .Where(s => s.Name.ToLower().Contains(searchText))
                //Выполняет запрос и материализует результаты в список 
                .ToList();

            SpidersDataGrid.ItemsSource = filtered;
        }
    }
}