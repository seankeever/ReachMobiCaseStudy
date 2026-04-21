PLACE THESE FILES IN YOUR PROJECT

1) Replace Program.cs with Program.cs from this folder.
2) Replace appsettings.json with the one in this folder, then add your NewsAPI key.
3) Add these new folders/files:
   - Controllers/NewsController.cs
   - Models/NewsSearchViewModel.cs
   - Models/NewsArticleViewModel.cs
   - Models/NewsSearchResultsViewModel.cs
   - Models/ArticleClickItemViewModel.cs
   - Models/ArticleClickStatsViewModel.cs
   - Services/INewsApiService.cs
   - Services/ISessionArticleTracker.cs
   - Services/NewsApiService.cs
   - Services/SessionArticleTracker.cs
   - Views/News/Index.cshtml
   - Views/News/Results.cshtml
   - Views/News/Stats.cshtml
4) Replace Views/Shared/_Layout.cshtml with the one in this folder.
5) Merge the CSS from wwwroot.site.css into your existing wwwroot/css/site.css file.
6) You can leave HomeController.cs, Privacy.cshtml, and ErrorViewModel.cs alone, but they will no longer be the main entry point.
7) Run the app. The default route now opens News/Index.
