# Tutorial: How to Upload Image Files from OrgChart JS to the server - ASP.NET Core

To upload image from the edit form you have to implement imageuploaded event hadler. Here is an example

```
chart.editUI.on('element-btn-click', function (sender, args) {
    OrgChart.fileUploadDialog(function (file) {
        var formData = new FormData();
        formData.append('files', file);

        fetch('/Home/UploadPhoto', {
            method: 'POST',
            body: formData
        })
            .then(response => {
                response.json().then(responseData => {
                    args.input.value = responseData.url;
                    sender.setAvatar(responseData.url);
                });
            })
            .catch((error) => {
                console.log(error)
            });;
    })
});
```


The next step is to create upload method on the server 
```
        [HttpPost]
        public async Task<IActionResult> UploadPhoto(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var file = files.First();
            var path = Path.Combine(_host.WebRootPath, "photos", file.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }            

            return Json(new
            {
                url = new Uri(new Uri(Request.Scheme + "://" + Request.Host.Value), Url.Content("~/photos/" + file.FileName)).ToString()
            });
        }
```


[BALKAN App](https://balkan.app)
