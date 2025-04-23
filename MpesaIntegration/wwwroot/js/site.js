// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(() => {
    $("#register").on('click', async (e) => {
        $("#register").text("Please Wait...").attr('disabled', true)
        const _response = await fetch("/register-urls");
        const response = await _response.json();

        $("#feedback").html(JSON.stringify(response))

        console.log(response)
        $("#register").text("Register URL")
            .removeAttr('disabled')
    })
})