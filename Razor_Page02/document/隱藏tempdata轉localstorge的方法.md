ASP.net core web應用程式有一個特性，button submit之後，當如果你的TempData存在，並且與第一次載入頁面時，資料有所不同，就會無法右鍵看到網頁原始碼。
此功能是以動態方式，讓客戶端與伺服器端持續連結，這對伺服器負擔來說，用戶越多，壓力就會越大，所以我希望在ASP.net core 6.0上實現Ajex技術。

我想把伺服器的負擔降到最低，把客戶端的負擔盡可能提高，這是我的目標，所以TempData必須要清掉，轉成localstorge。問題來了。這樣轉出去的過程，必須onload javascripte function，
localstorge又是輸入string的方式輸入。登入完後，把@TempData[]資料轉入，轉入又會因為先執行C#，後執行js的特性，
導致js的內容被看光光，我認為這樣並不安全。所以我要變一個魔術，我轉出去了，但也藏起來了。讓客戶端發生了甚麼事都不知道，神奇的掩蓋掉痕跡。怎麼做呢? 請慢慢看下去。

![image](https://github.com/light0986/ASP.NETcore6.0_Web/blob/main/Razor_Page02/document/1642148312773.jpg)

這是我設計的登入畫面。
很簡單，長這樣。

![image](https://github.com/light0986/ASP.NETcore6.0_Web/blob/main/Razor_Page02/document/1642150035658.jpg)

關鍵著眼點在這裡:<p id="err_text">@TempData["err_text"]</p>
他被放在button下方，但看不見。因為他現在根本不存在。TempData是以session的方式在做傳遞與暫存資訊的，因此當我輸入錯誤的帳號密碼，submit後資料會先被傳送到OnPost()

![image](https://github.com/light0986/ASP.NETcore6.0_Web/blob/main/Razor_Page02/document/1642148500654.jpg)

OnPost()會去訪問資料庫，最後回傳搜尋的結果。

![image](https://github.com/light0986/ASP.NETcore6.0_Web/blob/main/Razor_Page02/document/1642148512789.jpg)

當訪問結果回傳的是null值，TempData["err_text"]改變，reture Page();你就會看到，
button下方，出現了TempData["err_text"]的值，叫做"登入錯誤"
這時候你可以右鍵，檢視網頁原始碼，你會看到什麼?

![image](https://github.com/light0986/ASP.NETcore6.0_Web/blob/main/Razor_Page02/document/1642148327736.jpg)

![image](https://github.com/light0986/ASP.NETcore6.0_Web/blob/main/Razor_Page02/document/1642148337272.jpg)

你會看到它顯示錯誤。是!它顯示錯誤，可是你的頁面還能正常運行，這是一項很棒，可以善用的特性。
接著你可以順利登入。第一次登入，必定能看見後台資料，但我不想給別人看，所以我可以怎麼善用剛剛知道的特性呢?
Razor Page的特性是這樣，他在讀取cshtml的時候，先讀取C#語言的部分，然後再把html的結果傳給客戶看。
當TempData有值，你就看不到背景資料，但一直持續的連線，你的伺服器也會塞滿滿，很負擔。所以我要轉成localstorge，
之後就能用上Ajax的技術了。

![image](https://github.com/light0986/ASP.NETcore6.0_Web/blob/main/Razor_Page02/document/1642148364239.jpg)

![image](https://github.com/light0986/ASP.NETcore6.0_Web/blob/main/Razor_Page02/document/1642148567435.jpg)

登入的結果長這樣，你可以看到，我放上去了兩層if。第二層我習慣加的，預防萬一後續擴增用，重點就在第一層。
只要還存在TempData，我就可以看到中間這塊Dialog。接下來，就是見證奇蹟的時刻。右鍵，網頁原始碼。ㄟ!!!!!!

![image](https://github.com/light0986/ASP.NETcore6.0_Web/blob/main/Razor_Page02/document/1642148423119.jpg)

見鬼了吧? dialog標籤呢?
是的，因為TempData還有值，所以你看不到。
但dialog依然運行了，在dialog onload時，我已經把tempdata轉成localstorge。

![image](https://github.com/light0986/ASP.NETcore6.0_Web/blob/main/Razor_Page02/document/1642148529600.jpg)

並且你不點擊submit，進入這個可愛的OnPost()，然後刪光TempData，你的dialog會因為在最上層一直擋著，而什麼事情也不能做喔!
所以當按下去，就會重新刷新頁面，Tempdata也刪掉早轉成localstorge了。

![image](https://github.com/light0986/ASP.NETcore6.0_Web/blob/main/Razor_Page02/document/1642148589145.jpg)

![image](https://github.com/light0986/ASP.NETcore6.0_Web/blob/main/Razor_Page02/document/1642148614625.jpg)

![image](https://github.com/light0986/ASP.NETcore6.0_Web/blob/main/Razor_Page02/document/1642148447857.jpg)

你就會看到localstorge，已成為隱藏在你的Browser中的一個Token。然後就可以用iframe開始載入靜態網頁，設計，並使用Ajax功能囉~
