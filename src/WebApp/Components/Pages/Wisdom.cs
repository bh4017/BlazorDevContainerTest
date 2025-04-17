using WebApp.Models;

namespace WebApp.Components.Pages
{
    public partial class Wisdom
    {
        List<WisdomModel> perls = new List<WisdomModel>
        {
            new WisdomModel { Perl = "Work like hell.", Author = "Kapil Sachdeva" },
            new WisdomModel { Perl = "Knowledge is power.", Author = "Francis Bacon" },
            new WisdomModel { Perl = "The first principal is that you must not fool yourself.  And you are the easiest person to fool.", Author = "Richard Feynman" },
            new WisdomModel { Perl = "Do or do not, there is no try.", Author = "Yoda" }
        };

        private int currentIndex = 0;
        private string fadeState = "fade-in";
        private WisdomModel? currentPerl;

        protected override async Task OnInitializedAsync()
        {
            currentPerl = perls[currentIndex];
            _ = CycleQuotesAsync();
        }
        private async Task CycleQuotesAsync()
        {
            while (true)
            {
                fadeState = "fade-in";
                await InvokeAsync(StateHasChanged);

                await Task.Delay(3000);

                fadeState = "fade-out";
                await InvokeAsync(StateHasChanged);

                await Task.Delay(1000);

                currentIndex = (currentIndex + 1) % perls.Count;
                currentPerl = perls[currentIndex];

                fadeState = "fade-in";
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}