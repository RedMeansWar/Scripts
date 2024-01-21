const backgrounds = ["bg1.png", "bg2.png", "bg3.png", "bg4.png", "bg5.png", "bg6.png"];
let lastBackground = "";
let allowedDepts = [];
let numOfCharacters;

function chooseBg() {
  let index;
  do {
    index = Math.floor(Math.random() * backgrounds.length);
  } while (backgrounds[index] === lastBackground);

  lastBg = backgrounds[index];
  let imageUrl = `nui://core_framework/html/imgs/${lastBg}`;

  let bgImage = new Image();
  bgImage.src = imageUrl;

  $('body').css('background-image', `url("${imageUrl}")`);
}

// Framework Methods
function PlayAsCharacter(fn, ln, cash, bank, gender, department, dob) {
   $.post("https://core_framework/selectCharacter", JSON.stringify({
    firstName: fn,
    lastName: ln,
    gender: gender,
    cash: cash.toString(),
    bank: bank.toString(),
    dob: dob,
    department: department
   }));
}

function SetupAndDisplayErrorModal(error, title) {
  $('.modal').modal('hide');
  $('#errorMsg').text(error);
  $('.modal-title', $('#errorModal')).text(title);
  $('#errorModal').modal('show');
}

function CreateCharacter(fn, ln, cash, bank, gender, department, dob) {
  $('#createChar').modal('hide');

  $.post("https://core_framework/createCharacter", JSON.stringify({
    firstName: fn,
    lastName: ln,
    gender: gender,
    cash: cash.toString(),
    bank: bank.toString(),
    department: department,
    dob: dob
  }));
}

function ValidateCreateCharacter() {
  const emptyOrWhitespaceRegex = /^\s*$/;
  const alphabeticRegex = /^[A-Za-z]+$/;

  const firstNameInput = $('#createCharFirstName');
  const lastNameInput = $('#createCharLastName');
  const cashInput = $('#createCharCash');
  const bankInput = $('#createCharBank');
  const dobInput = $('#createCharDOB');
  const genderInput = $('#createCharGender');
  const departmentInput = $('#createCharDept');

  const firstName = firstNameInput.val().trim();
  const lastName = lastNameInput.val().trim();
  const cash = cashInput.val().trim();
  const bank = bankInput.val().trim();
  const dob = dobInput.val().trim();
  const gender = genderInput.val();
  const department = departmentInput.val();

  if (emptyOrWhitespaceRegex.test(firstName)) {
    SetupAndDisplayErrorModal('Please enter a first name for this character!', 'Error!');
    return;
  } else if (!alphabeticRegex.test(firstName)) {
    SetupAndDisplayErrorModal('First name should contain only alphabetic characters!', 'Error!');
    return;
  }

  if (emptyOrWhitespaceRegex.test(lastName)) {
    SetupAndDisplayErrorModal('Please enter a last name for this character!', 'Error!');
    return;
  } else if (!alphabeticRegex.test(lastName)) {
    SetupAndDisplayErrorModal('Last name should contain only alphabetic characters!', 'Error!');
    return;
  }

  const capitalizedFirstName = firstName.charAt(0).toUpperCase() + firstName.slice(1);
  const capitalizedLastName = lastName.charAt(0).toUpperCase() + lastName.slice(1);

  if (emptyOrWhitespaceRegex.test(cash)) {
    SetupAndDisplayErrorModal('Please enter a starting cash value between 0 and 15000 for this character!', 'Error!');
    return;
  } else if (isNaN(Number(cash)) || Number(cash) < 0 || Number(cash) > 15000) {
    SetupAndDisplayErrorModal('Starting cash value should be a number between 0 and 15000!', 'Error!');
    return;
  }

  if (emptyOrWhitespaceRegex.test(bank)) {
    SetupAndDisplayErrorModal('Please enter a starting bank value between 0 and 15000 for this character!', 'Error!');
    return;
  } else if (isNaN(Number(bank)) || Number(bank) < 0 || Number(bank) > 15000) {
    SetupAndDisplayErrorModal('Starting bank value should be a number between 0 and 15000!', 'Error!');
    return;
  }

  if (emptyOrWhitespaceRegex.test(dob) || isNaN(new Date(dob))) {
    SetupAndDisplayErrorModal('Please use the datepicker to choose a valid DoB for this character!', 'Error!');
    return;
  }

  const currentDate = new Date();
  const selectedDate = new Date(dob);

  if (selectedDate > currentDate) {
    SetupAndDisplayErrorModal('The selected date of birth is in the future!', 'Error!');
    return;
  }

  if (gender === null) {
    SetupAndDisplayErrorModal('Please select a valid gender for this character!', 'Error!');
    return;
  }

  if (department === null) {
    SetupAndDisplayErrorModal('Please select a valid department for this character!', 'Error!');
    return;
  }

  CreateCharacter(capitalizedFirstName, capitalizedLastName, cash, bank, gender, department, dob);
}

function DeleteCharacter(charId) {
  $('#confDeleteModal').modal('hide');
  $.post("https://core_framework/deleteCharacter", JSON.stringify({
    characterId: charId.toString()
  }));
}

function SetupDeleteCharacterModal(fn, ln, charid, dept) {
  $('#editChar').modal('hide');
  $('#confDeleteMessage').text(`Are you sure you wish to delete ${fn} ${ln} (${dept})? We won't be able to help you recover this character.`);
  $('#confDeleteModalFooter').empty();
  $('#confDeleteModalFooter').append('<button type="button" class="btn btn-danger mr-auto" onclick="DeleteCharacter(' + charid + ')">Delete</button>');
  $('#confDeleteModalFooter').append('<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>');
  $('#confDeleteModal').modal('show');
}

function ValidateEditCharacter() {
  const firstNameInput = $('#editCharFirstName');
  const lastNameInput = $('#editCharLastName');
  const dobInput = $('#editCharDOB');
  const genderInput = $('#editCharGender');
  const departmentInput = $('#editCharDept');

  const firstName = firstNameInput.val().trim();
  const lastName = lastNameInput.val().trim();
  const dob = dobInput.val().trim();
  const gender = genderInput.val();
  const department = departmentInput.val();

  const emptyOrWhitespaceRegex = /^\s*$/;
  const alphabeticRegex = /^[A-Za-z]+$/;

  if (emptyOrWhitespaceRegex.test(firstName)) {
    SetupAndDisplayErrorModal('Please enter a first name for this character!', 'Error!');
    return;
  } else if (!alphabeticRegex.test(firstName)) {
    SetupAndDisplayErrorModal('First name should contain only alphabetic characters!', 'Error!');
    return;
  }

  if (emptyOrWhitespaceRegex.test(lastName)) {
    SetupAndDisplayErrorModal('Please enter a last name for this character!', 'Error!');
    return;
  } else if (!alphabeticRegex.test(lastName)) {
    SetupAndDisplayErrorModal('Last name should contain only alphabetic characters!', 'Error!');
    return;
  }

  const capitalizedFirstName = firstName.charAt(0).toUpperCase() + firstName.slice(1);
  const capitalizedLastName = lastName.charAt(0).toUpperCase() + lastName.slice(1);

  if (emptyOrWhitespaceRegex.test(dob) || isNaN(new Date(dob))) {
    SetupAndDisplayErrorModal('Please use the datepicker to choose a valid DoB for this character!', 'Error!');
    return;
  }

  const currentDate = new Date();
  const selectedDate = new Date(dob);

  if (selectedDate > currentDate) {
    SetupAndDisplayErrorModal('The selected date of birth is in the future!', 'Error!');
    return;
  }

  if (emptyOrWhitespaceRegex.test(gender) || gender === null) {
    SetupAndDisplayErrorModal('Please select a valid gender for this character!', 'Error!');
    return;
  }

  if (emptyOrWhitespaceRegex.test(department) || department === null) {
    SetupAndDisplayErrorModal('Please select a valid department for this character!', 'Error!');
    return;
  }

  EditCharacter(capitalizedFirstName, capitalizedLastName, gender, department, dob);
}

function EditCharacter(fn, ln, gender, department, dob) {
  $('#editChar').modal('hide');

  $.post('https://core_framework/editCharacter', JSON.stringify({
    firstName: fn,
    lastName: ln,
    gender: gender,
    department: department,
    dob: dob,
    charId: $('#editCharIdHandlerHidden').val()
  }));
}

function SetupEditCharacterModal(firstName, lastName, dateOfBirth, gender, department, characterId) {
  const $editCharFirstName = $('#editCharFirstName');
  const $editCharLastName = $('#editCharLastName');
  const $editCharGender = $('#editCharGender');
  const $editCharDept = $('#editCharDept');
  const $editCharHeader = $('#editCharHeader');
  const $editCharDoB = $('#editCharDOB');
  const $editModalFooter = $('#editModalFooter');

  $editCharFirstName.val(firstName);
  $editCharLastName.val(lastName);
  $editCharHeader.text(`Editing: ${firstName} ${lastName} (${department})`);

  $editCharDoB.datepicker({
    format: 'mm/dd/yyyy',
    autoclose: true
  }).datepicker('setDate', new Date(dateOfBirth));

  $editCharDept.empty().append('<option selected disabled value="0">Select Department</option>');
  let deptOptions = allowedDepts.map(dept => `<option value="${dept}">${dept}</option>`).join('');
  $editCharDept.append(deptOptions).val(department.toUpperCase()).trigger('change');

  $editCharGender.empty().append('<option selected disabled>Select Gender</option>');
  const genderOptions = {
    'Male': 'Male',
    'Female': 'Female',
    'Other': 'Other'
  };
  const genderKeys = Object.keys(genderOptions);
  const genderOptionsHtml = genderKeys.map((key) => `<option value="${genderOptions[key]}">${key}</option>`).join('');
  $editCharGender.append(genderOptionsHtml).val(genderOptions[gender] || '').trigger('change');

  $('#editCharIdHandlerHidden').val(characterId);

  $editModalFooter.empty();
  $editModalFooter.append(`<button type="button" class="btn btn-danger" onclick="SetupDeleteCharacterModal('${firstName}', '${lastName}', '${characterId}', '${department}')">Delete Character</button>`);
  $editModalFooter.append('<button type="button" class="btn btn-primary" onclick="ValidateEditCharacter()">Save Character</button>');
  $editModalFooter.append('<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>');

  $('#editChar').modal('show');
}

function SetupCreateCharacterModal() {
  const $createCharFirstName = $('#createCharFirstName');
  const $createCharLastName = $('#createCharLastName');
  const $createCharCash = $('#createCharCash');
  const $createCharBank = $('#createCharBank');
  const $createCharGender = $('#createCharGender');
  const $createCharDept = $('#createCharDept');

  $createCharFirstName.val('');
  $createCharLastName.val('');
  $createCharCash.val('');
  $createCharBank.val('');
  $('#datepicker').val('').datepicker('update');
  $createCharGender.val('').trigger('change');

  $createCharDept.empty();
  $createCharDept.append('<option selected disabled value="0">Select Department</option>');

  for (let i = 0; i < allowedDepts.length; i++) {
    $createCharDept.append(`<option value="${allowedDepts[i]}">${allowedDepts[i]}</option>`);
  }

  $createCharDept.trigger('change');

  $('#createChar').modal('show');
}

function SetupCharacters(characters) {
  const $charList = $("#charList");
  $charList.empty();

  numOfCharacters = characters.length;

  characters.forEach(character => {
    let {
      Department,
      FirstName,
      LastName,
      CharacterId,
      DoB,
      Gender,
      Cash,
      Bank
    } = character;
    if (Department != null && Department != undefined) {
      if (Department == "CIV") Department = "Civ";

      const $listItem = $("<li>").addClass("characterItem");
      const $playButton = $("<button>")
        .addClass("btn btn-primary")
        .text(`Play As: ${FirstName} ${LastName} (${Department})`)
        .click(() => {
          PlayAsCharacter(FirstName, LastName, Cash, Bank, Gender, Department, DoB);
        });
      const $editButton = $("<button>")
        .addClass("btn btn-success")
        .text("Edit")
        .click(() => {
          SetupEditCharacterModal(FirstName, LastName, DoB, Gender, Department, CharacterId);
        });

      $listItem.append($playButton, $editButton);
      $charList.append($listItem);
    }
  });

  $("#createCharacterButton").remove();

  if (numOfCharacters < 12) {
    const createCharacterButton = $('<button>')
      .text(`New Character (${numOfCharacters}/12)`)
      .attr('id', 'createCharacterButton')
      .addClass('btn btn-primary')
      .click(function() {
        SetupCreateCharacterModal();
      });

    const $createCharacterContainer = $('<div>')
      .addClass('text-center')
      .append(createCharacterButton);

    $("#mainBody").append($createCharacterContainer);
  } else if (numOfCharacters < 0) {
    const createCharacterButton = $('<button>')
      .text(`New Character (0/12)`)
      .attr('id', 'createCharacterButton')
      .addClass('btn btn-primary')
      .click(function() {
        SetupCreateCharacterModal();
      });

    const $createCharacterContainer = $('<div>')
      .addClass('text-center')
      .append(createCharacterButton);
  }
}

function CancelNUI() {
  $.post("https://core_framework/cancelNUI");
}

function QuitGame() {
  $.post("https://core_framework/quitGame");
}

function Disconnect() {
  $.post("https://core_framework/disconnect");
}

function SpawnAtPrison() {
  $.post("https://core_framework/spawnAtPrison");
}

function SpawnAtGrapeseed() {
  $.post("https://core_framework/spawnAtGrapeseed");
}

function SpawnAtMotelNew() {
  $.post("https://core_framework/spawnAtMotelNew");
}

function SpawnAtMotel() {
  $.post("https://core_framework/spawnAtMotel");
}

function SpawnAtAbandonedMotel() {
  $.post("https://core_framework/spawnAtAbandonedMotel");
}

function SpawnAtCasino() {
  $.post("https://core_framework/spawnAtCasino");
}

function SpawnAtGroveStreet() {
  $.post("https://core_framework/spawnAtGroveStreet");
}

function SpawnAtMorningwoodHotel() {
  $.post("https://core_framework/spawnAtMorningwoodHotel");
}

function SpawnAtNikolaPlace() {
  $.post("https://core_framework/spawnAtNikolaPlace");
}

function SpawnAtStarLane() {
  $.post("https://core_framework/spawnAtStarLane");
}

function SpawnAtVinewoodPd() {
  $.post("https://core_framework/spawnAtVinewoodPd");
}

function SpawnAtSandyPd() {
  $.post("https://core_framework/spawnAtSandyPd");
}

function SpawnAtDavisPd() {
  $.post("https://core_framework/spawnAtDavisPd");
}

function SpawnAtPaletoPd() {
  $.post("https://core_framework/spawnAtPaletoPd");
}

function SpawnAtMissonRowPd() {
  $.post("https://core_framework/spawnAtMissionRowPd");
}

function SpawnAtRockfordPd() {
  $.post("https://core_framework/spawnAtRockfordPd");
}

function SpawnAtDelPerroPd() {
  $.post("https://core_framework/spawnAtDelPerroPd");
}

function HideCivSpawnModal() {
  $('#civSpawnModal').modal('hide');
}

function HideLeoSpawnModal() {
  $('#policeSpawnModal').modal('hide');
}

function DoNotTeleport() {
  $.post('https://core_framework/doNotTeleport');
}

// NUI Handlers
$(function() {
    window.addEventListener('message', function(event) {
      if (event.data.type === 'DISPLAY_NUI') {
        chooseBg();
        
        allowedDepts = event.data.departments;
      $('#titleHeader').text(event.data.aop);

      $('body').fadeIn(300, function() {
        $('body').css('display', 'block');
      });

      SetupCharacters(event.data.characters);

      $('#mainBody').css('display', 'block');
      } else if (event.data.type === 'CLOSE_UI') {
        $('body').fadeOut(300, function() {
          $('body').css('display', 'none');
        });
      } else if (event.data.type === 'SUCCESS') {
        $('#successMsg').text(event.data.msg);
        $('#successModal').modal('show');
      } else if (event.data.type === 'ERROR') {
        $('#errorMsg').text(event.data.msg);
        $('#errorModal').modal('show');
      } else if (event.data.type === 'UPDATE_AOP') {
        $('#titleHeader').text(event.data.aop);
      } else if (event.data.type === 'DISPLAY_SPAWN') {
        if (event.data.department === 'Civ') {
          $('#civSpawnModal').modal('show');
        } else if (event.data.department === 'SAHP' || 
        event.data.department === 'LSPD' || event.data.department === 'BCSO' ||
        event.data.department === 'SAHP' || event.data.department === 'LSFD') { // LSFD is temporart until I add more spawns.
          $('#policeSpawnModal').modal('show');
        }
      } else if (event.data.type === 'HIDE_SPAWN_MODALS') {
        $('#civSpawnModal').modal('hide');
        $('#policeSpawnModal').modal('hide');
      } else if (event.data.type === 'DONT_TELEPORT') {
        HideCivSpawnModal();
        HideLeoSpawnModal();
        $('body').css('display', 'none');
      } else if (event.data.type === 'COMMUNITY_NAME') {
        $('#communityHeader').text(event.data.commName);
      }
    });

    $('.datepicker').datepicker({
      format: 'mm/dd/yyyy',
      autoclose: true
    });
});