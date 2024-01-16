function displayNUI(bool) {
    if (bool === true) {
        $('#content-container').fadeIn(300, function() {
            $('#content-container').css('display', 'block');
        });
    } else {
        $('#content-container').fadeOut(300, function() {
            $('#content-container').css('display', 'block');
        });
    }
}

$(function() {
    window.addEventListener('message', function(event) {
        if (event.data.type === 'DISPLAY_NUI') {
            displayNUI(true);

            $('#licenseFirstName').text(event.data.firstName);
            $('#licenseLastName').text(event.data.lastName);
            $('#licenseDateOfBirth').text(event.data.dateOfBirth);
            $('#licenseGender').text(event.data.gender);
            $('#licenseWeight').text(event.data.weight);
            $('#licenseHeight').text(event.data.height);
            $('#licenseHair').text(event.data.hair);
            $('#licenseEyes').text(event.data.eyes);

        } else if (event.data.type === 'CLOSE_NUI') {
            displayNUI(false);
        }
    });

    document.onkeyup = function(data) {
        if (data.which === 27) { // escape key
            $.post('https://showid/closeNUI');
            return;
        }
    }
});