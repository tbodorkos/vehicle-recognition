# Vehicle Recognition
This is a learning application for me to try out Blazor, ML.NET Durable Functions
_Notes: The frontend project is only for testing connection between Blazor Web App and Functions/APIs, so no design applied._

### How does it work?
The application tries to predict if an image contains a train, a car or a ship.
- At first, you have to find an image on the internet, and get its URL.
- Then you can paste it into the input field of Recognition menu.
- This step invokes the function app and will run the train and prediction.
- The prediction result will be shown on site.

### Examples
- Train example URL: https://www.bud.hu/file/slides/2/2794/fe_780_454_mav_talent.jpg
- Car example URL: https://images.cnbctv18.com/wp-content/uploads/2023/02/04-BUGATTI_CHIRON-Profilee-1-780x438.jpg
- Ship example URL: https://c.ndtvimg.com/2022-12/6pldccqo_cruise-ship-mv-narrative-650_625x300_23_December_22.jpg

### TODO

- Handle error when invalid image input has been pasted.
