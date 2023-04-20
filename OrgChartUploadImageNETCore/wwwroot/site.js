
var chart = new OrgChart(document.getElementById("tree"), {
    mouseScrool: OrgChart.action.none,
    editForm: {
        photoBinding: "ImgUrl",
        elements: [
            { type: 'textbox', label: 'Photo Url', binding: 'ImgUrl', btn: 'Upload' },
        ]
    },
    nodeBinding: {
        field_0: "EmployeeName",
        field_1: "Title",
        img_0: "ImgUrl"
    }
});

chart.onInit(function () {
    this.editUI.show(1, false)
});

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

chart.load([
    { id: 1, EmployeeName: "Jack Hill", Title: "Chairman and CEO", Email: "amber@domain.com", ImgUrl: "https://cdn.balkan.app/shared/16.jpg", tags: ['orange'] },
]);