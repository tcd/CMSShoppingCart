// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(function() {

    let deleteButton = $("a.confirmDeletion")
    if (deleteButton.length) {
        deleteButton.click(() => {
            if (!confirm("Are you sure?")) { return false }
        })
    }  

    let alertDiv = $("div.alert.notification")
    if (alertDiv.length) {
        setTimeout(() => {
            alertDiv.fadeOut()
        }, 2000)
    } 

});

const readURL = function(input) {
    if (input.files && input.files[0]) {
        let reader = new FileReader();
        reader.onload = function(e) {
            $("img#imgpreview").attr("src", e.target.result).width(200).height(200);
        };
        reader.readAsDataURL(input.files[0]);
    }
}
