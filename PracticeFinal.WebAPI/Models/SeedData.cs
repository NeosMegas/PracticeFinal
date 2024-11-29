using Microsoft.EntityFrameworkCore;
using PracticeFinal.WebAPI.Models.SiteItems;

namespace PracticeFinal.WebAPI.Models
{
    public static class SeedData
    {
        public static void InitializeUsers(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationContext>>()))
            {
                if (!context.Users.Any())
                    context.Users.AddRange(
                        new User()
                        {
                            Roles = ["admin"],
                            Name = "admin",
                            Password = "admin",
                            Email = "admin@practicefinal"
                        }
                        );
                if (!context.MusketeerServices.Any())
                    context.MusketeerServices.AddRange(
                        new MusketeerService()
                        {
                            Name = "IT консалтинг",
                            Description = "Консультирование IT-специалистов в сфере изучения мушкетёрского мастерства.",
                            Position = 0
                        },
                        new MusketeerService()
                        {
                            Name = "Техническая поддержка",
                            Description = "Каждая внедренная ИТ-система нуждается в технической поддержке и дальнейшем согровождении. Консультации пользователей и администраторов систем, решение инцидентов любой сложности, исправление ошибок, быстрая реализации небольших запросоз на изменение, переход на новые версии системы, консультации по программно-аппаратному обеспечению и многие другие вопросы, которые возникают в процессе эксплуатации систем оперативно в круглосуточном режиме решаются сотрудниками выделенного функционального подразделения «Служба технической поддержки и сопровождения информационных систем».",
                            Position = 1
                        }
                        );
                if (!context.MusketeerProjects.Any())
                    context.MusketeerProjects.AddRange(
                        new MusketeerProject()
                        {
                            Name = "Сдача финальной работы в Skillbox",
                            Description = "Изучив все навыки на курсе \"C#–разработчик с нуля до PRO\", " +
                                          "был реализован проект данной системы для успешного завершения курса",
                            Image = "0001.png",
                            Position = 0
                        },
                        new MusketeerProject()
                        {
                            Name = "Создание IT-компании",
                            Description = "Несколько единомышленников — Атос, Портос и Арамис создали компанию #SkillProfi в сфере IT-консалтинга. Проект находится на этапе развития: есть несколько постоянных клиентов и регулярно появляются новые. В #SkillProfi все заявки поступают по телефону или от знакомых.",
                            Image = "86e82aa56f.png",
                            Position = 1
                        }
                        );
                if (!context.MusketeerBlogItems.Any())
                    context.MusketeerBlogItems.AddRange(
                        new MusketeerBlogItem()
                        {
                            Name = "Успешное завершение курса!",
                            Description = "Сдав финальную работу на курсе \"C#–разработчик с нуля до PRO\"," +
                                          "мы наконец-то его закончили и получили сертификат!😃",
                            Image = "0001.svg",
                            LargeImage = "0001.png",
                            Header = "УРА!!!",
                            Content = "Сдав финальную работу на курсе \"C#–разработчик с нуля до PRO\",мы наконец-то его закончили и получили сертификат!😃\nЭто было долгое путешествие, длиной в более, чем 2 года. Под конец было особенно тяжело, но мы сделали это!😎",
                            PublishDate = DateTime.Now,
                            Position = 0,
                        }
                        );
                if (!context.SiteInfos.Any())
                    context.SiteInfos.AddRange(
                        new SiteInfo()
                        {
                            StringId = "mainTitle",
                            Content = "IT-консалтинг от Трёх Мушкетёров!\n🤠🤠🤠"
                        },
                        new SiteInfo()
                        {
                            StringId = "useRandomSplash",
                            Content = "0"
                        },
                        new SiteInfo()
                        {
                            StringId = "splashText",
                            Content = "Всем привет!"
                        },
                        new SiteInfo()
                        {
                            StringId = "mainButtonText",
                            Content = "Главная"
                        },
                        new SiteInfo()
                        {
                            StringId = "projectsButtonText",
                            Content = "Проекты"
                        },
                        new SiteInfo()
                        {
                            StringId = "servicesButtonText",
                            Content = "Услуги"
                        },
                        new SiteInfo()
                        {
                            StringId = "blogButtonText",
                            Content = "Блог"
                        },
                        new SiteInfo()
                        {
                            StringId = "contactsButtonText",
                            Content = "Контакты"
                        },
                        new SiteInfo()
                        {
                            StringId = "contactAddress",
                            Content = "123456, г. Бобруйск, ул. Курвы, д. 1"
                        },
                        new SiteInfo()
                        {
                            StringId = "contactPhone",
                            Content = "+7 987 654 32 10"
                        },
                        new SiteInfo()
                        {
                            StringId = "contactEmail",
                            Content = "musketeer@skillprofi.pro"
                        },
                        new SiteInfo()
                        {
                            StringId = "contactName",
                            Content = "Атос, Портос и Арамис"
                        }

                        );
                if (!context.UserRequests.Any())
                    context.UserRequests.AddRange(
                        new UserRequest()
                        {
                            Name = "Люк Скайуокер",
                            MessageText = "Твои мысли тебя выдают, отец. Я чувствую в тебе добро, борьбу.",
                            RequestStatus = RequestStatus.NewRequest,
                            Email = "luke@skywalk.er",
                            Created = DateTime.Now
                        },
                        new UserRequest()
                        {
                            Name = "Оби-Ван Кеноби",
                            MessageText = "Всё кончено, Энакин! Я стою выше тебя!",
                            RequestStatus = RequestStatus.Completed,
                            Email = "obiwan@keno.bi",
                            Created = DateTime.Now.AddDays(-25)
                        },
                        new UserRequest()
                        {
                            Name = "Мастер Йода",
                            MessageText = "Слишком стар я. Слишком самонадеян, не видел, что прежний путь – не единственный. Те джедаи, учил которых я, чтобы стали такими, как те, что учили меня много столетий назад, в другое время живут. Изменилась Галактика. Но не видел этого я.",
                            RequestStatus = RequestStatus.InProgress,
                            Email = "yoda@grandmast.er",
                            Created = DateTime.Now.AddDays(-5)
                        },
                        new UserRequest()
                        {
                            Name = "Дарт Вейдер",
                            MessageText = "Я твой отец!",
                            RequestStatus = RequestStatus.NewRequest,
                            Email = "darthvader@deathdt.ar",
                            Created = DateTime.Now.AddDays(-1)
                        }
                        );
                context.SaveChanges();
            }
        }

    }
}
