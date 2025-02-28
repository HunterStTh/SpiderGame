28.02.25
MainWindow.xaml:
Добавил отображение TextBlock имени паука

MainWindow.xaml.cs:
Многократное переопределение контекстов окна было заменено единым  private readonly SpiderDbContext _dbContext = new SpiderDbContext(); используемым для всего класса окна
Добавил коменты
