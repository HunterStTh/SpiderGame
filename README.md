28.02.25
MainWindow.xaml:
Добавил отображение TextBlock имени паука

MainWindow.xaml.cs:
Многократное переопределение контекстов окна было заменено единым  private readonly SpiderDbContext _dbContext = new SpiderDbContext(); используемым для всего класса окна
Добавил коменты

11.03.25
MainWindow.xaml.cs:
Изменил событие смерти(был MessageBox) на labeltext с таймером отобржения 3 сек.
Добавил коменты
