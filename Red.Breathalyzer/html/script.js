function StartBreathalyzer() {
  $.post('https://breathalyzer/startTest');
}

function ResetBreathalyzer() {
  $.post('https://breathalyzer/resetTest');
}

function CancelBreathalyzerNUI() {
  $.post('https://breathalyzer/closeNUI');
}

$(function() {
  window.addEventListener('message', function(event) {
    if (event.data.type === 'DISPLAY_NUI') {
      $('#breathalyzer').fadeIn(300, function() {
        $('#breathalyzer').css('display', 'block');
      });
    } else if (event.data.type === 'CLOSE_NUI') {
      $('#breathalyzer').fadeOut(300, function() {
        $('#breathalyzer').css('display', 'none');
      });
    } else if (event.data.type === 'RESET_NUI') {
      $('#baclevel').text('0.00');
    } else if (event.data.type === 'UPDATE_BAC') {
      $('#baclevel').text(event.data.level);
    }
  });

  document.onkeyup = function(data) {
    if (data.which === 27) {
      $.post('https://breathalyzer/closeNUI');
      return;
    }
  };
});