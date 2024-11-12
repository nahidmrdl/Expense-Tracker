using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        public DashboardController(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<ActionResult> Index()
        {
            // Last 7 days 
            DateTime StartDate = DateTime.Now.AddDays(-6);
            DateTime EndDate = DateTime.Now;

            List<Transaction> SelectedTransactions = await _context.Transactions.Include(x => x.Category).Where(y => y.Date >= StartDate && y.Date <= EndDate).ToListAsync();

            //Total Income for last 7 days

            decimal TotalIncome = SelectedTransactions.Where(x => x.Category.Type == "Income").Sum(y => y.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("N2") + "₼";

            //Total Expense for last 7 days

            decimal TotalExpense = SelectedTransactions.Where(x => x.Category.Type == "Expense").Sum(y => y.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("N2") + "₼";

            //Total Balance for last 7 days

            decimal TotalBalance = TotalIncome - TotalExpense;
            string formattedBalance = TotalBalance < 0
                ? $"-{Math.Abs(TotalBalance).ToString("N2") + "₼"}"
                : TotalBalance.ToString("N2") + "₼";

            ViewBag.TotalBalance = formattedBalance;

            // Last 30 days
            // Last 30 days
            DateTime StartDate30 = DateTime.Now.AddDays(-29);
            DateTime EndDate30 = DateTime.Now;

            List<Transaction> SelectedTransactions30 = await _context.Transactions.Include(x => x.Category)
                .Where(y => y.Date >= StartDate30 && y.Date <= EndDate30)
                .ToListAsync();




            //Total Income for last 30 days

            decimal TotalIncome30 = SelectedTransactions30.Where(x => x.Category.Type == "Income").Sum(y => y.Amount);
            ViewBag.TotalIncome30 = TotalIncome30.ToString("N2") + "₼";

            //Total Expense for last 30 days

            decimal TotalExpense30 = SelectedTransactions30.Where(x => x.Category.Type == "Expense").Sum(y => y.Amount);
            ViewBag.TotalExpense30 = TotalExpense30.ToString("N2") + "₼";

            //Total Balance for last 30 days

            decimal TotalBalance30 = TotalIncome30 - TotalExpense30;
            string formattedBalance30 = TotalBalance30 < 0
                ? $"-{Math.Abs(TotalBalance30).ToString("N2") + "₼"}"
                : TotalBalance30.ToString("N2") + "₼";

            ViewBag.TotalBalance30 = formattedBalance30;



            // Last 12 months

            DateTime StartDate12 = DateTime.Now.AddMonths(-11);
            DateTime EndDate12 = DateTime.Now;

            List<Transaction> SelectedTransactions12 = await _context.Transactions.Include(x => x.Category).Where(y => y.Date >= StartDate12 && y.Date <= EndDate12).ToListAsync();

            // Monthly Income and Expenses
            var monthlyData = SelectedTransactions12
                .GroupBy(t => new { Year = t.Date.Year, Month = t.Date.Month })
                .Select(g => new
                {
                    MonthYear = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    Income = g.Where(t => t.Category.Type == "Income").Sum(t => t.Amount),
                    Expense = g.Where(t => t.Category.Type == "Expense").Sum(t => t.Amount)
                })
                .OrderBy(d => d.MonthYear)
                .ToList();

            ViewBag.MonthlyData = monthlyData;

            //Total Income for last 12 months

            decimal TotalIncome12 = SelectedTransactions12.Where(x => x.Category.Type == "Income").Sum(y => y.Amount);
            ViewBag.TotalIncome12 = TotalIncome12.ToString("N2") + "₼";

            //Total Expense for last 12 months

            decimal TotalExpense12 = SelectedTransactions12.Where(x => x.Category.Type == "Expense").Sum(y => y.Amount);
            ViewBag.TotalExpense12 = TotalExpense12.ToString("N2") + "₼";

            //Total Balance for last 12 months

            decimal TotalBalance12 = TotalIncome12 - TotalExpense12;
            string formattedBalance12 = TotalBalance12 < 0
                ? $"-{Math.Abs(TotalBalance12).ToString("N2") + "₼"}"
                : TotalBalance12.ToString("N2") + "₼";

            ViewBag.TotalBalance12 = formattedBalance12;




            //Doughnut Chart - Expense By Category
            ViewBag.DoughnutChartData = SelectedTransactions30
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = "₼" + k.Sum(j => j.Amount).ToString("N2")
                })
                .OrderByDescending(l => l.amount)
                .ToList();



            //Recent Transactions
            ViewBag.RecentTransactions = await _context.Transactions
                .Include(i => i.Category)
                .OrderByDescending(j => j.Date)
                .Take(11)
                .ToListAsync();



            // Total Income and Expense for all time
            decimal totalIncomeAllTime = await _context.Transactions
                .Where(x => x.Category.Type == "Income")
                .SumAsync(y => y.Amount);

            decimal totalExpenseAllTime = await _context.Transactions
                .Where(x => x.Category.Type == "Expense")
                .SumAsync(y => y.Amount);

            decimal totalBalanceAllTime = totalIncomeAllTime - totalExpenseAllTime;


            ViewBag.TotalBalance = totalBalanceAllTime;

            var tags = new[] { "motivational", "success", "life", "business" };
            var quotes = new List<Quote>();
            var random = new Random();

            foreach (var tag in tags)
            {
                try
                {
                    // Fetch a random quote for each tag
                    var response = await _httpClient.GetStringAsync($"https://api.quotable.io/random?tags={tag}");
                    var quoteData = JsonConvert.DeserializeObject<dynamic>(response);

                    if (quoteData != null)
                    {
                        quotes.Add(new Quote
                        {
                            Content = quoteData.content,
                            Author = quoteData.author
                        });
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Log or handle the exception
                    Console.WriteLine($"Error fetching quote for tag {tag}: {ex.Message}");
                }
            }

            // Ensure we have quotes to select from
            if (quotes.Any())
            {
                // Randomly select one quote from the list
                var selectedQuote = quotes[random.Next(quotes.Count)];
                ViewBag.MotivationalQuote = new
                {
                    QuoteText = selectedQuote.Content,
                    Author = selectedQuote.Author
                };
            }
            else
            {
                // Handle the case where no quotes are available
                ViewBag.MotivationalQuote = new
                {
                    QuoteText = "No quotes available.",
                    Author = ""
                };
            }

            return View();


        }

    }
    public class Quote
    {
        public string Content { get; set; }
        public string Author { get; set; }
    }


    public class SplineChartData
    {
        public string day;
        public decimal income;
        public decimal expense;

    }

    public class incomeData
    {
        public string day;
        public decimal income;
    }

    public class expenseData
    {
        public string day;
        public decimal expense;
    }
}
