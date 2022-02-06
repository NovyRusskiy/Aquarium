using System;
using System.Collections.Generic;

namespace Aquarium
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isWork = true;
            int id = 1;

            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();

            List<Fish> fishes = new List<Fish>()
            {
                new Fish(id++, "Окунь", 14, 2),
                new Fish(id++, "Карп", 10, 3),
                new Fish(id++, "Сом", 8, 4),
                new Fish(id++, "Скалярия", 12, 5),
                new Fish(id++, "Морской конёк", 11, 6),
            };

            Aquarium aquarium = new Aquarium(fishes);

            while (isWork)
            {
                Console.Clear();

                Console.WriteLine("Это Ваш аквариум, в нем отображаются его обитатели.");
                Console.WriteLine("не забывайте кормить Ваших рыбок, чтобы они оставались живы.\n");
                Console.WriteLine("Чтобы перемотать время на следующий день, нажмите - ENTER");
                Console.WriteLine("Для того, чтобы покормить рыбок нажмит - TAB");
                Console.WriteLine("Чтобы добавить новую рыбу в аквариум, нажмите - SPACE");
                Console.WriteLine("Для того, чтобы убрать рыбу, нажмите - DELETE");
                Console.WriteLine("Если вы хотите выйти из программы, нажмите - ESCAPE\n");

                aquarium.PrintInfo();
                ConsoleKeyInfo key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        aquarium.FinishDay();
                        break;
                    case ConsoleKey.Tab:
                        aquarium.FeedInhabitants();
                        break;
                    case ConsoleKey.Spacebar:
                        aquarium.AddFish();
                        break;
                    case ConsoleKey.Delete:
                        aquarium.DeleteFish();
                        break;
                    case ConsoleKey.Escape:
                        isWork = false;
                        break;
                    default:
                        PrintErrorMessage();
                        break;
                }
            }
        }

        static void PrintErrorMessage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nНеверная команда");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
        }
    }

    public interface ICloneable
    {
        object Clone();
    }

    class Aquarium
    {
        private List<Fish> _fishes;
        private List<Fish> _inhabitants;
        private int _days;

        public Aquarium(List<Fish> fishes)
        {
            _fishes = fishes;
            _inhabitants = new List<Fish>(0);
            _days = 1;
        }

        public void AddFish()
        {
            Console.WriteLine("\n$$$");
            foreach (Fish fish in _fishes)
            {
                Console.Write(fish.ID + ". ");
                fish.ShowInfo();
                Console.WriteLine($" - необходимо кормить раз в {fish.MaxHungryDays} дней");
            }

            Console.WriteLine("\nВыберите рыбку, которую хотите добавить в свой аквариум: ");
            bool isFind = FindFish(_fishes, out int inputNumber);

            if (isFind)
            {
                foreach (Fish fish in _fishes)
                {
                    if (inputNumber == fish.ID)
                    {
                        _inhabitants.Add((Fish)fish.Clone());
                        break;
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nУвы! Такой рыбки нет.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
            }
        }

        public void DeleteFish()
        {
            if (_inhabitants.Count > 0)
            {
                Console.WriteLine("\nВыберите рыбку, которую хотите убрать из аквариума: ");
                bool isFind = FindFish(_inhabitants, out int serialNumber);

                if (isFind)
                {
                    Fish selectedFish = null;

                    foreach (Fish fish in _inhabitants)
                    {
                        if (serialNumber == _inhabitants.IndexOf(fish) + 1)
                        {
                            selectedFish = fish;
                            break;
                        }
                    }

                    _inhabitants.Remove(selectedFish);
                }
                else
                {
                    PrintErrorMessage();
                }
            }
            else
            {
                Console.WriteLine("\nУ Вас пока нет рыбок");
                Console.ReadKey();
            }
        }

        public void FeedInhabitants()
        {
            if (_inhabitants.Count > 0)
            {
                foreach (Fish fish in _inhabitants)
                {
                    fish.Feed();
                }

                Console.WriteLine("\nРыбки успешно покормлены");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("\nУ Вас пока нет рыбок");
                Console.ReadKey();
            }
        }

        public void FinishDay()
        {
            if (_inhabitants.Count > 0)
            {
                foreach (Fish fish in _inhabitants)
                {
                    fish.ChangeCondition();
                }
                DeleteDeadFish();
            }

            _days++;
        }

        public void PrintInfo()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{_days} день\n\nВаш аквариум:");

            foreach (Fish fish in _inhabitants)
            {
                Console.Write($"{_inhabitants.IndexOf(fish) + 1}. ");
                fish.ShowInfo();
                Console.WriteLine($" (дней без еды {fish.HungryDays}/{fish.MaxHungryDays})");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        private bool FindFish(List<Fish> fishList, out int inputNumber)
        {
            string userInput = Console.ReadLine();
            bool success = int.TryParse(userInput, out inputNumber);
            bool isHave = inputNumber > 0 && inputNumber <= fishList.Count;

            return isHave && success;
        }

        private void DeleteDeadFish()
        {
            bool isHaveDead = true;

            while (isHaveDead && _inhabitants.Count > 0)
            {
                Fish deadFish = null;
                int lastFishIndex = _inhabitants.Count - 1;
                Fish lastFish = _inhabitants[lastFishIndex];

                foreach (Fish fish in _inhabitants)
                {
                    if (fish.IsDead)
                    {
                        deadFish = fish;
                        break;
                    }
                    else if (lastFish.IsDead == false)
                    {
                        isHaveDead = false;
                    }
                }

                _inhabitants.Remove(deadFish);
            }
        }

        private void PrintErrorMessage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nТакой рыбки нет в списке");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
        }
    }

    class Fish : ICloneable
    {
        private int _longevity;
        private int _age;
        private int _id;
        private string _name;
        private int _hungryDays;
        private int _maxHungryDays;

        public Fish(int id, string name, int longevity, int maxHungruDays)
        {
            _id = id;
            _name = name;
            _longevity = longevity;
            _age = 0;
            _hungryDays = 0;
            _maxHungryDays = maxHungruDays;
        }

        public int ID => _id;

        public int Longevity => _longevity;

        public int HungryDays => _hungryDays;

        public int MaxHungryDays => _maxHungryDays;

        public bool IsDead => _age > _longevity || _hungryDays > _maxHungryDays;

        public object Clone()
        {
            return new Fish(_id, _name, _longevity, _maxHungryDays);
        }

        public void ShowInfo()
        {
            Console.Write($"{_name} - {_age}");
        }

        public void ChangeCondition()
        {
            _age++;
            _hungryDays++;
        }

        public void Feed()
        {
            _hungryDays = 0;
        }
    }
}
