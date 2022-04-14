using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using System.IO;

namespace LastTelegramBot
{
    class Program
    {
        static TelegramBotClient Bot;
        static void Main(string[] args)
        {
            Bot = new TelegramBotClient("666239340:AAEAoyyxm5LAA2ZQhftnknspp8gxfW0vTVo");

            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();

            Console.ReadKey();
        }

        private static async void BotOnCallbackQueryReceived(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            string buttonText = e.CallbackQuery.Data;
            string ID = $"{e.CallbackQuery.From.Id}";
            string Name = $"{e.CallbackQuery.From.FirstName}";
            string LName = $"{e.CallbackQuery.From.LastName}";
            char symb = buttonText.ElementAt(0);
            try
            {
                await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Товар \"{buttonText}\" добавлен в твое меню");
                FileStream file = new FileStream("D:\\HellperBK/check" + e.CallbackQuery.From.Id + ".txt", FileMode.Append);
                StreamWriter writer = new StreamWriter(file);
                writer.Write(symb);
                writer.Close();
                Console.WriteLine($"{Name} {LName} добавил в меню {buttonText}");
            }
            catch
            {

            }
        }

        private static async void BotOnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;
            if (message == null || message.Type != MessageType.Text) return;
            string name = $"{message.From.Id}";
            string Name = $"{message.From.FirstName}";
            string LName = $"{message.From.LastName}";
            if (message.Text == "/start")
            {
                if (System.IO.File.Exists("D:\\HellperBK/info" + message.From.Id + ".txt"))
                {
                    await Bot.SendTextMessageAsync(message.From.Id, "Мы же уже знакомились. Если нужна какая-то информация, загляни в \"помощь\"");
                    Console.WriteLine("Файл info" + message.From.Id + ".txt уже существует");
                }
                else
                {
                    FileStream file = new FileStream("D:\\HellperBK/info" + message.From.Id + ".txt", FileMode.Append);
                    StreamWriter writer = new StreamWriter(file);
                    writer.WriteLine(message.From.Id);
                    writer.Close();
                    await Bot.SendTextMessageAsync(message.From.Id, "Привет! Я KylieBK, призвана помочь тебе с выбором в Бургер Кинге. Как все работает? Смотри ниже.");
                    await Bot.SendTextMessageAsync(message.From.Id, "Чтобы набрать товаров в корзину, напиши мне \"заказ\" или \"Заказ\". Мне без разницы, большой ли будет первая буква. Последний заказ сохраняется, поэтому, если ты не хочешь менять корзину, просто вводи бюджет.");
                    await Bot.SendPhotoAsync(message.From.Id, "https://i.imgur.com/8NXCako.jpg");
                    await Bot.SendTextMessageAsync(message.From.Id, "Клацай по тому, что тебе хочется. Помни: 1 клик - 1 товар; 2 клика - 2 товара. Когда выберешь, вводи мне свой бюджет. Например, \"148\". Я наберу самых дорогих возможных товаров. Кстати, приоритет выбираешь ты - порядок выбора товаров влияет на их важность. Если тебе не хватит денег на все, я об этом напишу.");
                    await Bot.SendPhotoAsync(message.From.Id, "https://i.imgur.com/AylfFnT.jpg");
                    await Bot.SendTextMessageAsync(message.From.Id, "Вот и все. Введи \"Помощь\", и у тебя появится кнопка быстрого доступа к командам. И не забудь чекнуть разрабов. Приятного пользования.");
                    Console.WriteLine("Создан файл info" + message.From.Id + ".txt и записан ChatID");
                }
            }
            else if (message.Text == "заказ" || message.Text == "Заказ")
            {
                System.IO.File.Delete("D:\\HellperBK/check" + message.From.Id + ".txt");
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Говяжий бургер")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Куриный бургер")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Ролл")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Фри картофель")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Деревенский картофель")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Закуска")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Шейк")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Напиток")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Соус")
                        }
                });
                await Bot.SendTextMessageAsync(message.From.Id, "Добавляй товары себе в корзину:", replyMarkup: inlineKeyboard);
                await Bot.SendTextMessageAsync(message.From.Id, "После выбора товаров введи мне свой бюджет, чтобы я набрала тебе заказ. Например: \"282\"");
            }
            else if (message.Text.All(char.IsDigit) && System.IO.File.Exists("D:\\HellperBK/check" + message.From.Id + ".txt"))
            {
                int cnt = 0;
                cnt = Convert.ToInt32(message.Text);
                if (cnt <= 7000 && cnt > 0)
                {
                    char[] listgoods = System.IO.File.ReadAllText("D:\\HellperBK/check" + message.From.Id + ".txt").ToCharArray();//Все символы товаров
                    string[] checklist = new string[listgoods.Length]; //Все покупки
                    int[] pricelist = new int[listgoods.Length];//Все цены
                    int[] passlist = new int[listgoods.Length];//Все пропуска
                    for (int i = 0; i < passlist.Length; i++) { passlist[i] = 0; }
                    for (int i = 0; i < pricelist.Length; i++) { pricelist[i] = 0; }
                    int countgoods = 0;
                    int allprice = 0;
                    int lvl = 0;
                    bool t = false;
                    int k = 1;
                    for (int n = 0; n < listgoods.Length; n++) { Console.Write(listgoods[n]); Console.WriteLine(); } //
                    while (t != true)
                    {
                        for (int i = 0; i < listgoods.Length; i++)
                        {
                            //
                            if (listgoods[i] == 'Г' && lvl < 14)
                            {
                                Console.WriteLine("Начало Г");
                                string[] arStr = System.IO.File.ReadAllLines("D:\\HellperBK/Говяжий бургер.txt");
                                int price = Convert.ToInt32(arStr[lvl].Remove(arStr[lvl].IndexOf("|")));
                                string title = arStr[lvl].Substring(arStr[lvl].IndexOf("|") + 1);
                                if (allprice + price - pricelist[i] <= cnt)
                                {
                                    Console.WriteLine(allprice);
                                    allprice = allprice + price - pricelist[i];
                                    checklist[i] = title;
                                    pricelist[i] = price;
                                    goto FinishFor;
                                }
                                else
                                {
                                    passlist[i] = 1; goto FinishFor;
                                }
                            }
                            else if (listgoods[i] == 'Г' && lvl >= 14) { passlist[i] = 1; goto FinishFor; }
                            //
                            if (listgoods[i] == 'К' && lvl < 4)
                            {
                                Console.WriteLine("Начало К");
                                string[] arStr = System.IO.File.ReadAllLines("D:\\HellperBK/Куриный бургер.txt");
                                int price = Convert.ToInt32(arStr[lvl].Remove(arStr[lvl].IndexOf("|")));
                                string title = arStr[lvl].Substring(arStr[lvl].IndexOf("|") + 1);
                                if (allprice + price - pricelist[i] <= cnt)
                                {
                                    Console.WriteLine(allprice);
                                    allprice = allprice + price - pricelist[i];
                                    checklist[i] = title;
                                    pricelist[i] = price;
                                    goto FinishFor;
                                }
                                else
                                {
                                    passlist[i] = 1; goto FinishFor;
                                }
                            }
                            else if (listgoods[i] == 'К' && lvl >= 4) { passlist[i] = 1; goto FinishFor; }
                            //
                            if (listgoods[i] == 'Р' && lvl < 4)
                            {
                                Console.WriteLine("Начало Р");
                                string[] arStr = System.IO.File.ReadAllLines("D:\\HellperBK/Ролл.txt");
                                int price = Convert.ToInt32(arStr[lvl].Remove(arStr[lvl].IndexOf("|")));
                                string title = arStr[lvl].Substring(arStr[lvl].IndexOf("|") + 1);
                                if (allprice + price - pricelist[i] <= cnt)
                                {
                                    Console.WriteLine(allprice);
                                    allprice = allprice + price - pricelist[i];
                                    checklist[i] = title;
                                    pricelist[i] = price;
                                    goto FinishFor;
                                }
                                else
                                {
                                    passlist[i] = 1; goto FinishFor;
                                }
                            }
                            else if (listgoods[i] == 'Р' && lvl >= 4) { passlist[i] = 1; goto FinishFor; }
                            //
                            if (listgoods[i] == 'Ф' && lvl < 3)
                            {
                                Console.WriteLine("Начало Ф");
                                string[] arStr = System.IO.File.ReadAllLines("D:\\HellperBK/Фри картофель.txt");
                                int price = Convert.ToInt32(arStr[lvl].Remove(arStr[lvl].IndexOf("|")));
                                string title = arStr[lvl].Substring(arStr[lvl].IndexOf("|") + 1);
                                if (allprice + price - pricelist[i] <= cnt)
                                {
                                    Console.WriteLine(allprice);
                                    allprice = allprice + price - pricelist[i];
                                    checklist[i] = title;
                                    pricelist[i] = price;
                                    goto FinishFor;
                                }
                                else
                                {
                                    passlist[i] = 1; goto FinishFor;
                                }
                            }
                            else if (listgoods[i] == 'Ф' && lvl >= 3) { passlist[i] = 1; goto FinishFor; }
                            //
                            if (listgoods[i] == 'Д' && lvl < 2)
                            {
                                Console.WriteLine("Начало К");
                                string[] arStr = System.IO.File.ReadAllLines("D:\\HellperBK/Деревенский картофель.txt");
                                int price = Convert.ToInt32(arStr[lvl].Remove(arStr[lvl].IndexOf("|")));
                                string title = arStr[lvl].Substring(arStr[lvl].IndexOf("|") + 1);
                                if (allprice + price - pricelist[i] <= cnt)
                                {
                                    Console.WriteLine(allprice);
                                    allprice = allprice + price - pricelist[i];
                                    checklist[i] = title;
                                    pricelist[i] = price;
                                    goto FinishFor;
                                }
                                else
                                {
                                    passlist[i] = 1; goto FinishFor;
                                }
                            }
                            else if (listgoods[i] == 'Д' && lvl >= 2) { passlist[i] = 1; goto FinishFor; }
                            //
                            if (listgoods[i] == 'З' && lvl < 9)
                            {
                                Console.WriteLine("Начало З");
                                string[] arStr = System.IO.File.ReadAllLines("D:\\HellperBK/Закуска.txt");
                                int price = Convert.ToInt32(arStr[lvl].Remove(arStr[lvl].IndexOf("|")));
                                string title = arStr[lvl].Substring(arStr[lvl].IndexOf("|") + 1);
                                if (allprice + price - pricelist[i] <= cnt)
                                {
                                    Console.WriteLine(allprice);
                                    allprice = allprice + price - pricelist[i];
                                    checklist[i] = title;
                                    pricelist[i] = price;
                                    goto FinishFor;
                                }
                                else
                                {
                                    passlist[i] = 1; goto FinishFor;
                                }
                            }
                            else if (listgoods[i] == 'З' && lvl >= 9) { passlist[i] = 1; goto FinishFor; }
                            //
                            if (listgoods[i] == 'Ш' && lvl < 3)
                            {
                                Console.WriteLine("Начало Ш");
                                string[] arStr = System.IO.File.ReadAllLines("D:\\HellperBK/Шейк.txt");
                                int price = Convert.ToInt32(arStr[lvl].Remove(arStr[lvl].IndexOf("|")));
                                string title = arStr[lvl].Substring(arStr[lvl].IndexOf("|") + 1);
                                if (allprice + price - pricelist[i] <= cnt)
                                {
                                    Console.WriteLine(allprice);
                                    allprice = allprice + price - pricelist[i];
                                    checklist[i] = title;
                                    pricelist[i] = price;
                                    goto FinishFor;
                                }
                                else
                                {
                                    passlist[i] = 1; goto FinishFor;
                                }
                            }
                            else if (listgoods[i] == 'Ш' && lvl >= 3) { passlist[i] = 1; goto FinishFor; }
                            //
                            if (listgoods[i] == 'Н' && lvl < 1)
                            {
                                Console.WriteLine("Начало Н");
                                string[] arStr = System.IO.File.ReadAllLines("D:\\HellperBK/Напиток.txt");
                                int price = Convert.ToInt32(arStr[lvl].Remove(arStr[lvl].IndexOf("|")));
                                string title = arStr[lvl].Substring(arStr[lvl].IndexOf("|") + 1);
                                if (allprice + price - pricelist[i] <= cnt)
                                {
                                    Console.WriteLine(allprice);
                                    allprice = allprice + price - pricelist[i];
                                    checklist[i] = title;
                                    pricelist[i] = price;
                                    goto FinishFor;
                                }
                                else
                                {
                                    passlist[i] = 1; goto FinishFor;
                                }
                            }
                            else if (listgoods[i] == 'Н' && lvl >= 1) { passlist[i] = 1; goto FinishFor; }
                            //
                            if (listgoods[i] == 'С' && lvl < 3)
                            {
                                Console.WriteLine("Начало С");
                                string[] arStr = System.IO.File.ReadAllLines("D:\\HellperBK/Соус.txt");
                                int price = Convert.ToInt32(arStr[lvl].Remove(arStr[lvl].IndexOf("|")));
                                string title = arStr[lvl].Substring(arStr[lvl].IndexOf("|") + 1);
                                if (allprice + price - pricelist[i] <= cnt)
                                {
                                    Console.WriteLine(allprice);
                                    allprice = allprice + price - pricelist[i];
                                    checklist[i] = title;
                                    pricelist[i] = price;
                                    goto FinishFor;
                                }
                                else
                                {
                                    passlist[i] = 1; goto FinishFor;
                                }
                            }
                            else if (listgoods[i] == 'С' && lvl >= 3) { passlist[i] = 1; goto FinishFor; }
                        ///////
                        FinishFor:;
                            Console.WriteLine(passlist[i]);
                            k = 1;
                            for (int p = 0; p < passlist.Length; p++)
                            {
                                k = k * passlist[p];
                            }
                            Console.WriteLine(k);
                            if (k == 1) { t = true; goto FinishWhile; }
                        }
                        lvl++;
                    //t = true;
                    FinishWhile:;
                    }
                    if (checklist[0] != null)
                    {
                        float baks = 67.97f;
                        var flot = (float)Math.Round(allprice / baks, 2);
                        await Bot.SendTextMessageAsync(message.From.Id, "Держи заказ на сумму " + allprice + "р (" + flot + "$) :");
                        for (int i = 0; i < checklist.Length; i++)
                        {
                            if (checklist[i] != null)
                            {
                                countgoods++;
                                await Bot.SendTextMessageAsync(message.From.Id, "" + checklist[i]);
                            }
                        }
                        if (countgoods < listgoods.Length)
                        {
                            await Bot.SendTextMessageAsync(message.From.Id, "Денег на всё не хватило. Я набрала, что смогла.");
                        }
                        flot = (float)Math.Round((cnt - allprice) / baks, 2);
                        await Bot.SendTextMessageAsync(message.From.Id, "Проверь сдачу: " + (cnt - allprice) + "р (" + flot + "$)");
                    }
                    else { await Bot.SendTextMessageAsync(message.From.Id, "Денег не хватает даже на самый дешёвый заказ."); }

                }
                else if (cnt > 7000)
                {
                    await Bot.SendTextMessageAsync(message.From.Id, "Тебе хватит денег? Проверь получше.");
                }
                else if (cnt <= 0)
                {
                    await Bot.SendTextMessageAsync(message.From.Id, "Иди стреляй чирики.");
                }
            }
            
            else if (message.Text.All(char.IsDigit) && !System.IO.File.Exists("D:\\HellperBK/check" + message.From.Id + ".txt"))
            {
                await Bot.SendTextMessageAsync(message.From.Id, "Твоя корзина пуста.");
            }
            else if (message.Text == "разработчики" || message.Text == "Разработчики")
            {
                var developers = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("Программист", "https://vk.com/kealfeyne")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("Автор идеи", "https://vk.com/kirshv")
                    }
                });
                await Bot.SendTextMessageAsync(message.From.Id, "Создатели HellperBK:", replyMarkup: developers);
            }
            else if (message.Text == "помощь" || message.Text == "Помощь")
            {
                var replyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    new[]
                    {
                        new KeyboardButton("Заказ"),
                    },
                    new[]
                    {
                        new KeyboardButton("Разработчики")
                    }
                })
                {
                    ResizeKeyboard = true
                };
                await Bot.SendTextMessageAsync(message.From.Id, "Доступные команды.", replyMarkup: replyKeyboard);
            }

            Console.WriteLine($"{Name} {LName} сообщил \"{message.Text}\"");
        }
    }
}
//string[] arStr = System.IO.File.ReadAllLines("D:\\HellperBK/Бургер.txt");
//int price = Convert.ToInt32(arStr[lvlb].Remove(arStr[lvlb].IndexOf("|")));
//string title = arStr[lvlb].Substring(arStr[lvlb].IndexOf("|") + 1);
