// Гаврилов Д
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace SpiderGame
{
    // Класс SpiderFilterConverter реализует интерфейс IMultiValueConverter
    public class SpiderFilterConverter : IMultiValueConverter
    {
        // Метод Convert выполняет преобразование данных
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Проверяем, что передано достаточно значений
            if (values.Length < 2)
                return null;

            // Первый аргумент - коллекция пауков
            var spiders = values[0] as ObservableCollection<Spider>;

            // Второй аргумент - выбранный паук (цель)
            var selectedTarget = values[1] as Spider;

            // Если коллекция и выбранный паук существуют
            if (spiders != null && selectedTarget != null)
            {
                // Фильтруем коллекцию, исключая выбранного паука
                return spiders.Where(spider => spider != selectedTarget).ToList();
            }

            // Если фильтрация не удалась, возвращаем исходную коллекцию
            return spiders;
        }

        // Метод ConvertBack не реализован, так как преобразование в обратную сторону не требуется
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}