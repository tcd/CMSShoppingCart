// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(function() {

    let deleteButton = $("a.confirmDeletion")
    if (deleteButton.length) {
        deleteButton.click(() => {
            if (!confirm("Are you sure?")) { return false }
        })
    }  

    let notification = $("div.alert.notification")
    if (notification.length) {
        setTimeout(() => {
            notification.fadeOut()
        }, 2000)
    } 

})
