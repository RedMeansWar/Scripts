const backgrounds = ["bg1.png", "bg2.png", "bg3.png", "bg4.png", "bg5.png", "bg6.png"]; // List of the allowed background format.
let lastBackground = ""; // last background selected is empty. Preparing for background.
let allowedDepts = [];
let numOfCharacters; // Number of characters displayed.
let spawnLocations = {
  prison: "https://core_framework/spawnAtPrison",
  grapeseed: "https://core_framework/spawnAtGrapeseed",
  new_motel: "https://core_framework/spawnAtMotelNew",
  motel: "https://core_framework/spawnAtMotel",
  abandoned_motel: "https://core_framework/spawnAtAbandonedMotel",
  casino: "https://core_framework/spawnAtCasino",
  grove_street: "https://core_framework/spawnAtGroveStreet",
  morningwood_hotel: "https://core_framework/spawnAtMorningwoodHotel",
  nikola_place: "https://core_framework/spawnAtNikolaPlace",
  star_lane: "https://core_framework/spawnAtStarLane",
  vinewood_pd: "https://core_framework/spawnAtVinewoodPd",
  sandy_pd: "https://core_framework/spawnAtSandyPd",
  davis_pd: "https://core_framework/spawnAtDavisPd",
  paleto_pd: "https://core_framework/spawnAtPaletoPd",
  mission_row_pd: "https://core_framework/spawnAtMissionRowPd",
  rockford_pd: "https://core_framework/spawnAtRockfordPd",
  delperro_pd: "https://core_framework/spawnAtDelPerroPd",
  firedept_s1: "https://core_framework/spawnAtStation1",
  firedept_s2: "https://core_framework/spawnAtStation2",
  firedept_s3: "https://core_framework/spawnAtStation3",
  firedept_s4: "https://core_framework/spawnAtStation4",
  firedept_s5: "https://core_framework/spawnAtStation5",
  firedept_s6: "https://core_framework/spawnAtStation6",
  firedept_s7: "https://core_framework/spawnAtStation7",
  firedept_s8: "https://core_framework/spawnAtStation8"
}

/**
 * Randomly selects a new background image and sets it as the body background.
 * Ensures the new background is different from the previously used one.
 */
function chooseBg() {
  let index;

  // Keep generating random indices until a different background is found.
  do {
    index = Math.floor(Math.random() * backgrounds.length);
  } while (backgrounds[index] === lastBackground);

  // Update the last used background record.
  lastBg = backgrounds[index];

  // Construct the image URL.
  let imageUrl = `nui://core_framework/html/imgs/${lastBg}`;

  // Pre-load the image to prevent display delays.
  let bgImage = new Image();
  bgImage.src = imageUrl;

  // Set the background image for the body element.
  $('body').css('background-image', `url("${imageUrl}")`);
}

function hideModal(modalId) {
  $(`#${modalId}`).modal('hide');
}

function showModal(modalId) {
  $(`#${modalId}`).modal('show');
}

function postToFramework(action) {
  $.post(`https://core_framework/${action}`)
}

function spawnAt(location) {
  $.post(spawnLocations[location]);
}

/**
 * Sends a request to select the specified character for gameplay.
 *
 * @param {string} fn Character's first name.
 * @param {string} ln Character's last name.
 * @param {number} cash Character's cash value.
 * @param {number} bank Character's bank value.
 * @param {string} gender Character's gender.
 * @param {string} department Character's department.
 * @param {string} dob Character's date of birth.
 */
function playAsCharacter(fn, ln, cash, bank, gender, department, dob) {
  // Send a POST request to the server to select the character.
   $.post("https://core_framework/selectCharacter", JSON.stringify({
    firstName: fn,
    lastName: ln,
    gender: gender,
    cash: cash.toString(), // Ensure cash and bank values are strings for JSON serialization
    bank: bank.toString(),
    dob: dob,
    department: department
   }));
}

/**
 * Prepares and displays an error modal with the provided error message and title.
 *
 * @param {string} error The error message to display.
 * @param {string} title The title for the error modal.
 */
function setupAndDisplayErrorModal(error, title) {
  // Hide any currently open modals.
  $('.modal').modal('hide');
  
  // Set the error message and title in the error modal elements.
  $('#errorMsg').text(error);
  $('.modal-title', $('#errorModal')).text(title);
  
  // Show the error modal.
  showModal('errorModal');
}

/**
 * Sends a request to create a new character with the provided information.
 *
 * @param {string} fn Character's first name.
 * @param {string} ln Character's last name.
 * @param {number} cash Character's starting cash value.
 * @param {number} bank Character's starting bank value.
 * @param {string} gender Character's gender.
 * @param {string} department Character's department.
 * @param {string} dob Character's date of birth.
 */
function createCharacter(fn, ln, cash, bank, gender, department, dob) {
  // Hide the create character modal
  hideModal('createChar');

  // Send a POST request to the server with the character data.
  $.post("https://core_framework/createCharacter", JSON.stringify({
    firstName: fn,
    lastName: ln,
    gender: gender,
    cash: cash.toString(), // Ensure cash and bank values are strings for JSON serialization.
    bank: bank.toString(),
    department: department,
    dob: dob
  }));
}

/**
 * Validates user input in the create character form and calls the create function if valid.
 */
function validateCreateCharacter() {
  // Regex patterns for validation.
  const emptyOrWhitespaceRegex = /^\s*$/;
  const alphabeticRegex = /^[A-Za-z]+$/;

  // Get references to form elements.
  const firstNameInput = $('#createCharFirstName');
  const lastNameInput = $('#createCharLastName');
  const cashInput = $('#createCharCash');
  const bankInput = $('#createCharBank');
  const dobInput = $('#createCharDOB');
  const genderInput = $('#createCharGender');
  const departmentInput = $('#createCharDept');

  // Get form values and trim any leading/trailing whitespace.
  const firstName = firstNameInput.val().trim();
  const lastName = lastNameInput.val().trim();
  const cash = cashInput.val().trim();
  const bank = bankInput.val().trim();
  const dob = dobInput.val().trim();
  const gender = genderInput.val();
  const department = departmentInput.val();

  // Validate first name, last name, and date of birth (similar to previous functions).
  if (emptyOrWhitespaceRegex.test(firstName)) {
    setupAndDisplayErrorModal('Please enter a first name for this character!', 'Error!');
    return;
  } else if (!alphabeticRegex.test(firstName)) {
    setupAndDisplayErrorModal('First name should contain only alphabetic characters!', 'Error!');
    return;
  }

  if (emptyOrWhitespaceRegex.test(lastName)) {
    setupAndDisplayErrorModal('Please enter a last name for this character!', 'Error!');
    return;
  } else if (!alphabeticRegex.test(lastName)) {
    setupAndDisplayErrorModal('Last name should contain only alphabetic characters!', 'Error!');
    return;
  }

  const capitalizedFirstName = firstName.charAt(0).toUpperCase() + firstName.slice(1);
  const capitalizedLastName = lastName.charAt(0).toUpperCase() + lastName.slice(1);

  // Validate cash value.
  if (emptyOrWhitespaceRegex.test(cash)) {
    setupAndDisplayErrorModal('Please enter a starting cash value between 0 and 15000 for this character!', 'Error!');
    return;
  } else if (isNaN(Number(cash)) || Number(cash) < 0 || Number(cash) > 15000) {
    setupAndDisplayErrorModal('Starting cash value should be a number between 0 and 15000!', 'Error!');
    return;
  }

  // Validate bank value (similar to cash validation).
  if (emptyOrWhitespaceRegex.test(bank)) {
    setupAndDisplayErrorModal('Please enter a starting bank value between 0 and 15000 for this character!', 'Error!');
    return;
  } else if (isNaN(Number(bank)) || Number(bank) < 0 || Number(bank) > 15000) {
    setupAndDisplayErrorModal('Starting bank value should be a number between 0 and 15000!', 'Error!');
    return;
  }

  // Validate gender date of birth, and department (similar to previous functions).
  if (emptyOrWhitespaceRegex.test(dob) || isNaN(new Date(dob))) {
    setupAndDisplayErrorModal('Please use the datepicker to choose a valid DoB for this character!', 'Error!');
    return;
  }

  const currentDate = new Date();
  const selectedDate = new Date(dob);

  if (selectedDate > currentDate) {
    setupAndDisplayErrorModal('The selected date of birth is in the future!', 'Error!');
    return;
  }

  if (gender === null) {
    setupAndDisplayErrorModal('Please select a valid gender for this character!', 'Error!');
    return;
  }

  if (department === null) {
    setupAndDisplayErrorModal('Please select a valid department for this character!', 'Error!');
    return;
  }

  // If all validations pass, call the createCharacter function with the validated data.
  createCharacter(capitalizedFirstName, capitalizedLastName, cash, bank, gender, department, dob);
}

function deleteCharacter(charId) {
  // Hide the edit character modal if it's open.
  hideModal('confDeleteModal');

  // Send a POST request to the server to delete the character.
  $.post("https://core_framework/deleteCharacter", JSON.stringify({
    characterId: charId.toString()
  }));
}

/**
 * Prepares and displays a confirmation modal for deleting a character.
 *
 * @param {string} fn Character's first name.
 * @param {string} ln Character's last name.
 * @param {number} charid Character's unique identifier.
 * @param {string} dept Character's department.
 */
function setupDeleteCharacterModal(fn, ln, charid, dept) {
  // Hide the edit character modal if it's open.
  hideModal('editChar');
  
  // Populate the confirmation message with character details.
  $('#confDeleteMessage').text(`Are you sure you wish to delete ${fn} ${ln} (${dept})? We won't be able to help you recover this character.`);

  // Build the confirmation modal footer with buttons.
  $('#confDeleteModalFooter').empty();
  $('#confDeleteModalFooter').append('<button type="button" class="btn btn-danger mr-auto" onclick="DeleteCharacter(' + charid + ')">Delete</button>');
  $('#confDeleteModalFooter').append('<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>');

  // Show the confirmation modal.
  showModal('confDeleteModal');
}

/**
 * Validates user input in the edit character form and calls the edit function if valid.
 */
function validateEditCharacter() {
  // Get references to form elements.
  const firstNameInput = $('#editCharFirstName');
  const lastNameInput = $('#editCharLastName');
  const dobInput = $('#editCharDOB');
  const genderInput = $('#editCharGender');
  const departmentInput = $('#editCharDept');

  // Get form values and trim any leading/trailing whitespace.
  const firstName = firstNameInput.val().trim();
  const lastName = lastNameInput.val().trim();
  const dob = dobInput.val().trim();
  const gender = genderInput.val();
  const department = departmentInput.val();

  // Regex patterns for validation.
  const emptyOrWhitespaceRegex = /^\s*$/;
  const alphabeticRegex = /^[A-Za-z]+$/;

  // Validate first name.
  if (emptyOrWhitespaceRegex.test(firstName)) {
    setupAndDisplayErrorModal('Please enter a first name for this character!', 'Error!');
    return; // Exit the function if validation fails.
  } else if (!alphabeticRegex.test(firstName)) {
    setupAndDisplayErrorModal('First name should contain only alphabetic characters!', 'Error!');
    return;
  }

  // Validate last name (similar to first name validation).
  if (emptyOrWhitespaceRegex.test(lastName)) {
    setupAndDisplayErrorModal('Please enter a last name for this character!', 'Error!');
    return;
  } else if (!alphabeticRegex.test(lastName)) {
    setupAndDisplayErrorModal('Last name should contain only alphabetic characters!', 'Error!');
    return;
  }

  // Capitalize the first letter of first and last names.
  const capitalizedFirstName = firstName.charAt(0).toUpperCase() + firstName.slice(1);
  const capitalizedLastName = lastName.charAt(0).toUpperCase() + lastName.slice(1);

  // Validate date of birth.
  if (emptyOrWhitespaceRegex.test(dob) || isNaN(new Date(dob))) {
    setupAndDisplayErrorModal('Please use the datepicker to choose a valid DoB for this character!', 'Error!');
    return;
  }

  const currentDate = new Date();
  const selectedDate = new Date(dob);
  
  if (selectedDate > currentDate) {
    setupAndDisplayErrorModal('The selected date of birth is in the future!', 'Error!');
    return;
  }

  // Validate gender.
  if (emptyOrWhitespaceRegex.test(gender) || gender === null) {
    setupAndDisplayErrorModal('Please select a valid gender for this character!', 'Error!');
    return;
  }

  // Validate department.
  if (emptyOrWhitespaceRegex.test(department) || department === null) {
    setupAndDisplayErrorModal('Please select a valid department for this character!', 'Error!');
    return;
  }

  // If all validations pass, call the editCharacter function with the validated data.
  editCharacter(capitalizedFirstName, capitalizedLastName, gender, department, dob);
}

/**
 * Edit characters with user defined information.
 *
 * @param {string} fn Character's first name.
 * @param {string} ln Character's last name.
 * @param {string} dob Character's date of birth.
 * @param {string} gender Character's gender.
 * @param {string} department Character's department.
 */
function editCharacter(fn, ln, gender, department, dob) {
  // Hide the edit character modal.
  hideModal('editChar');

  // Send a POST request to the server to update the character information.
  $.post('https://core_framework/editCharacter', JSON.stringify({
    // Character details to be updated:
    firstName: fn, // Get first name from a variable named 'fn'.
    lastName: ln, // Get last name from a variable named 'ln'.
    gender: gender, // Get gender from a variable named 'gender'.
    department: department, // Get department from a variable named 'department'.
    dob: dob, // Get date of birth from a variable named 'dob'.
    charId: $('#editCharIdHandlerHidden').val() // Get character ID from a hidden input field.
  }));
}

/**
 * Prepares and displays the edit character modal with pre-populated information.
 *
 * @param {string} firstName Character's first name.
 * @param {string} lastName Character's last name.
 * @param {string} dateOfBirth Character's date of birth.
 * @param {string} gender Character's gender.
 * @param {string} department Character's department.
 * @param {number} characterId Character's unique identifier.
 */
function setupEditCharacterModal(firstName, lastName, dateOfBirth, gender, department, characterId) {
  // Get references to form elements within the modal.
  const $editCharFirstName = $('#editCharFirstName');
  const $editCharLastName = $('#editCharLastName');
  const $editCharGender = $('#editCharGender');
  const $editCharDept = $('#editCharDept');
  const $editCharHeader = $('#editCharHeader');
  const $editCharDoB = $('#editCharDOB');
  const $editModalFooter = $('#editModalFooter');

  // Fill form fields with the provided character information.
  $editCharFirstName.val(firstName);
  $editCharLastName.val(lastName);
  $editCharHeader.text(`Editing: ${firstName} ${lastName} (${department})`);

  // Reset and populate the department dropdown.
  $editCharDoB.datepicker({
    format: 'mm/dd/yyyy',
    autoclose: true
  }).datepicker('setDate', new Date(dateOfBirth));

  // Reset and populate the gender dropdown.
  $editCharDept.empty().append('<option selected disabled value="0">Select Department</option>');
  let deptOptions = allowedDepts.map(dept => `<option value="${dept}">${dept}</option>`).join('');
  $editCharDept.append(deptOptions).val(department.toUpperCase()).trigger('change');

  // Store character ID for internal use.
  $editCharGender.empty().append('<option selected disabled>Select Gender</option>');
  const genderOptions = {
    'Male': 'Male',
    'Female': 'Female',
    'Other': 'Other'
  };
  const genderKeys = Object.keys(genderOptions);
  const genderOptionsHtml = genderKeys.map((key) => `<option value="${genderOptions[key]}">${key}</option>`).join('');
  $editCharGender.append(genderOptionsHtml).val(genderOptions[gender] || '').trigger('change');

  // Store character ID for internal use.
  $('#editCharIdHandlerHidden').val(characterId);

  // Dynamically build the modal footer with buttons.
  $editModalFooter.empty();
  $editModalFooter.append(`<button type="button" class="btn btn-danger" onclick="setupDeleteCharacterModal('${firstName}', '${lastName}', '${characterId}', '${department}')">Delete Character</button>`);
  $editModalFooter.append('<button type="button" class="btn btn-primary" onclick="validateEditCharacter()">Save Character</button>');
  $editModalFooter.append('<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>');

  $('#editChar').modal('show');
  showModal('editChar');
}

/**
 * Prepares and displays the create character modal.
 */
function setupCreateCharacterModal() {
  // Get references to form elements within the modal.
  const $createCharFirstName = $('#createCharFirstName');
  const $createCharLastName = $('#createCharLastName');
  const $createCharCash = $('#createCharCash');
  const $createCharBank = $('#createCharBank');
  const $createCharGender = $('#createCharGender');
  const $createCharDept = $('#createCharDept');

  // Clear any existing values in the form fields.
  $createCharFirstName.val('');
  $createCharLastName.val('');
  $createCharCash.val('');
  $createCharBank.val('');
  $('#datepicker').val('').datepicker('update'); // Clear and update the date picker.
  $createCharGender.val('').trigger('change'); // Clear and trigger a change event for the gender dropdown.

  // Reset and populate the department dropdown.
  $createCharDept.empty();  // Clear existing options.

  $createCharDept.append('<option selected disabled value="0">Select Department</option>'); // Add a placeholder option.

  // Add options for allowed departments.
  for (let i = 0; i < allowedDepts.length; i++) {
    $createCharDept.append(`<option value="${allowedDepts[i]}">${allowedDepts[i]}</option>`);
  }

  $createCharDept.trigger('change'); // // Trigger a change event to potentially update other elements.

  // Show the create character modal.
  showModal('createChar');
}

/**
 * Populates the character list with available characters.
 *
 * @param {Object[]} characters An array of character objects.
 */
function setupCharacters(characters) {
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
          playAsCharacter(FirstName, LastName, Cash, Bank, Gender, Department, DoB);
        });
      const $editButton = $("<button>")
        .addClass("btn btn-success")
        .text("Edit")
        .click(() => {
          setupEditCharacterModal(FirstName, LastName, DoB, Gender, Department, CharacterId);
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
        setupCreateCharacterModal();
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
        setupCreateCharacterModal();
      });

    const $createCharacterContainer = $('<div>')
      .addClass('text-center')
      .append(createCharacterButton);
  }
}

function cancelNUI() {
  postToFramework("cancelNUI"); // cancels the framework's UI.
}

function quitGame() {
  postToFramework("quitGame"); // Quits the player's game.
}

function disconnect() {
  postToFramework("disconnect"); // Drops the player from the server.
}

function spawnAtPrison() {
  spawnAt("prison");
}

function spawnAtGrapeseed() {
  spawnAt("grapeseed");
}

function spawnAtMotelNew() {
  spawnAt('new_motel');
}

function spawnAtMotel() {
  spawnAt("motel");
}

function spawnAtAbandonedMotel() {
  spawnAt("abandoned_motel");
}

function spawnAtCasino() {
  spawnAt("casino");
}

function spawnAtGroveStreet() {
  spawnAt("grove_street");
}

function spawnAtMorningwoodHotel() {
  spawnAt("morningwood_hotel");
}

function spawnAtNikolaPlace() {
  spawnAt("nikola_place");
}

function spawnAtStarLane() {
  spawnAt("star_lane");
}

function spawnAtVinewoodPd() {
  spawnAt("vinewood_pd");
}

function spawnAtSandyPd() {
  spawnAt("sandy_pd");
}

function spawnAtDavisPd() {
  spawnAt("davis_pd");
}

function spawnAtPaletoPd() {
  spawnAt("paleto_pd")
}

function spawnAtMissonRowPd() {
  spawnAt("mission_row_pd");
}

function spawnAtRockfordPd() {
  spawnAt("rockford_pd")
}

function spawnAtDelPerroPd() {
  spawnAt("delperro_pd")
}

function spawnAtStation1() {
  spawnAt("firedept_s1");
}

function spawnAtStation2() {
  spawnAt("firedept_s2");
}

function spawnAtStation3() {
  spawnAt("firedept_s3");
}

function spawnAtStation4() {
  spawnAt("firedept_s4");
}

function spawnAtStation5() {
  spawnAt("firedept_s5");
}

function spawnAtStation6() {
  spawnAt("firedept_s6");
}

function spawnAtStation7() {
  spawnAt("firedept_s7");
}

function spawnAtStation8() {
  spawnAt("firedept_s8");
}

function hideFireSpawnModal() {
  hideModal("fireSpawnModal"); // Hides the spawns for the fire department.
}

function hideCivSpawnModal() {
  hideModal("civSpawnModal"); // Hides the spawns for civilians.
}

function hideLeoSpawnModal() {
  hideModal("policeSpawnModal"); // Hides the spawns for the police.
}

function doNotTeleport() {
  postToFramework("doNotTeleport"); // Doesn't teleport to any spawn location.
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

      setupCharacters(event.data.characters);

      $('#mainBody').css('display', 'block');
      } else if (event.data.type === 'CLOSE_UI') {
        $('body').fadeOut(300, function() {
          $('body').css('display', 'none');
        });
      } else if (event.data.type === 'SUCCESS') {
        $('#successMsg').text(event.data.msg);
        showModal('successModal');
      } else if (event.data.type === 'ERROR') {
        $('#errorMsg').text(event.data.msg);
        showModal('errorModal');
      } else if (event.data.type === 'UPDATE_AOP') {
        $('#titleHeader').text(event.data.aop);
      } else if (event.data.type === 'DISPLAY_SPAWN') {
        if (event.data.department === 'Civ') {
          showModal('civSpawnModal');
        } else if (event.data.department === 'SAHP' || 
        event.data.department === 'LSPD' || event.data.department === 'BCSO' ||
        event.data.department === 'SAHP') { // LSFD is temporary until I add more spawns.
          showModal('policeSpawnModal');
        }
        else if (event.data.department === 'LSFD') {
          showModal('fireSpawnModal');
        }
      } else if (event.data.type === 'HIDE_SPAWN_MODALS') {
        hideModal('civSpawnModal');
        hideModal('policeSpawnModal');
        hideModal('fireSpawnModal');
      } else if (event.data.type === 'DONT_TELEPORT') {
        hideModal('civSpawnModal');
        hideModal('policeSpawnModal');
        hideModal('fireSpawnModal');
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