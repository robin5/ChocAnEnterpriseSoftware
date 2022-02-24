// Transaction states
const STATE_INPUT_PROVIDER_ID = 0;
const STATE_INPUT_MEMBER_ID = 1;
const STATE_INPUT_SERVICE_DATE = 2;
const STATE_INPUT_SERVICE_CODE = 3;
const STATE_ACCEPT_OR_REJECT_SERVICE_CODE = 4;
const STATE_ENTER_COMMENT_AND_SUBMIT_TRANSACTION = 5;
const STATE_WAIT_FOR_ENTER = 6;

// Transaction fields
let providerId;
let providerName;
let memberId;
let memberStatus;
let serviceId;
let serviceDate;
let serviceName;
let serviceCost;
let serviceComment;

// Set first state
let state = STATE_INPUT_PROVIDER_ID;
let transactionTimeout;

// Once the page is loaded, immediately execute the following
(function () {

    // Attach an event listener to handle input from the form
    document
        .getElementById('input-form')
        .addEventListener("submit", event => onSubmit(event));
})();

/// <summary>
/// Submit event handler for input box
/// </summary>
function onSubmit(event) {

    // Prevent default submit operation
    event.preventDefault();

    // Get trimmed text from input box
    let input = document.getElementById('input-element').value;
    if (null != input)
        input = input.trim();

    // Ignore Blank input accept for the following states:
    //   - STATE_ENTER_COMMENT_AND_SUBMIT_TRANSACTION
    //   - STATE_WAIT_FOR_ENTER
    if (!input) {
        if ((state == STATE_ENTER_COMMENT_AND_SUBMIT_TRANSACTION) ||
            (state = STATE_WAIT_FOR_ENTER)) {
            input = "";
        } else {
            return;
        }
    }

    // Execute the state
    switch (state) {

        case STATE_INPUT_PROVIDER_ID: // submit provider ID
            FetchProviderId(input);
            break;

        case STATE_INPUT_MEMBER_ID:
            FetchMemberId(input);
            break;

        case STATE_INPUT_SERVICE_DATE:
            StopTransactionTimeout();
            EnterServiceDate(input);
            break;

        case STATE_INPUT_SERVICE_CODE:
            FetchServiceCode(input);
            break;

        case STATE_ACCEPT_OR_REJECT_SERVICE_CODE:
            AcceptOrRejectServiceCode(input);
            break;

        case STATE_ENTER_COMMENT_AND_SUBMIT_TRANSACTION:
            EnterServiceComment(input);
            PostTransaction();
            break;

        case STATE_WAIT_FOR_ENTER:
            ResetStateForNextTransaction();
            break;
    }

    // Clear input line for next input
    ClearInputline();
}

function ClearInputline() {
    document.getElementById('input-element').value = "";
}

function StartTransactionTimeout() {
    transactionTimeout = setTimeout(HandleTransactionTimeout, 60000);
}

function StopTransactionTimeout() {
    clearTimeout(transactionTimeout);
}

function HandleTransactionTimeout() {
    ResetStateForNextTransaction();
    ClearInputline();
}

function FetchProviderId(id) {

    let href = `https://localhost:44380/api/terminal/provider/${id}`;

    fetch(href)
        .then(response => DecodeJsonResponse(response))
        .then(data => HandleProviderIdResponse(data))
        .catch(error => HandleError(error));
}

function HandleProviderIdResponse(data) {

    DisplayJson(data);
    if (data) {
        providerId = data.id;
        providerName = data.name;
        DisplayInstructions("Enter member number.");
        state = STATE_INPUT_MEMBER_ID;
    } else {
        DisplayInstructions("Provider number not found, re-enter provider number.");
        state = STATE_INPUT_PROVIDER_ID;
    }
}

function FetchMemberId(id) {

    let href = `https://localhost:44317/api/terminal/member/${id}`;

    fetch(href)
        .then(response => DecodeJsonResponse(response))
        .then(data => HandleMemberResponse(data))
        .catch(error => HandleError(error));
}

function HandleMemberResponse(data) {

    DisplayJson(data);
    if (data) {
        memberId = data.id;
        memberStatus = data.status;
        if (data.status == null || data.status.toLowerCase() != "active") {
            DisplayInstructions("Member is not active. Press <strong>enter</strong> to continue.");
            state = STATE_WAIT_FOR_ENTER;
        } else {
            DisplayInstructions("Enter service date: MM–DD–YYYY");
            StartTransactionTimeout();
            state = STATE_INPUT_SERVICE_DATE;
        }
    } else {
        DisplayInstructions("Member number not found. Press <strong>enter</strong> to continue.");
        state = STATE_WAIT_FOR_ENTER;
    }
}

function EnterServiceDate(input) {

    serviceDate = (new Date(input)).toISOString();

    console.log(`Service Date: ${serviceDate}`)

    DisplayInstructions("Enter service code.");
    state = STATE_INPUT_SERVICE_CODE;
}

function FetchServiceCode(id) {

    let href = `https://localhost:44317/api/terminal/service/${id}`;

    fetch(href)
        .then(response => DecodeJsonResponse(response))
        .then(data => HandelServiceCodeResponse(data))
        .catch(error => HandleError(error));
}

function HandelServiceCodeResponse(data) {

    DisplayJson(data);
    if (data) {
        serviceId = data.id;
        serviceName = data.name;
        serviceCost = data.cost;
        DisplayInstructions(`<strong>${serviceName}</strong>: enter <strong>Yes</strong> to accept service code or <strong>No</strong> to re-enter service code`);
        state = STATE_ACCEPT_OR_REJECT_SERVICE_CODE;
    } else {
        DisplayInstructions("Service code not found, re-enter service code.");
        state = STATE_INPUT_SERVICE_CODE;
    }
    console.log(`Service code: ${serviceId}`);
}

function AcceptOrRejectServiceCode(text) {

    let answer = text.toLowerCase();

    if ((answer == "yes") || (answer == "y")) {
        DisplayInstructions("Enter comment about service or just press enter for no comment");
        state = STATE_ENTER_COMMENT_AND_SUBMIT_TRANSACTION;
    } else if ((answer == "no") || (answer == "n")) {
        DisplayInstructions("Re-enter service code...");
        state = STATE_INPUT_SERVICE_CODE;
    } else {
        DisplayInstructions(`Did not understand answer. <strong>${serviceName}</strong>: enter <strong>Yes</strong> to accept service code or <strong>No</strong> to re-enter service code`);
        state = STATE_ACCEPT_OR_REJECT_SERVICE_CODE;
    }
}

function EnterServiceComment(text) {
    serviceComment = text;
}

function PostTransaction() {

    let transaction = {};
    transaction.providerId = providerId;
    transaction.memberId = memberId;
    transaction.serviceId = serviceId;
    transaction.serviceDate = serviceDate;
    transaction.serviceComment = serviceComment;

    console.log(`Post Transaction:`)
    console.log(transaction)

    let href = "https://localhost:44317/api/terminal/transaction";

    fetch(href, {
        method: "POST",
        headers: new Headers({ "content-type": "application/json" }),
        body: JSON.stringify(transaction)
    })
        .then(response => DecodeJsonResponse(response))
        .then(data => HandleTransactionResponse(data))
        .catch(error => HandleError(error));
}

function HandleTransactionResponse(data) {

    DisplayJson(data);

    if (data) {
        DisplayInstructions(`Transaction accepted: Service cost $${serviceCost}. Press <strong>enter</strong> to continue.`);
        state = STATE_WAIT_FOR_ENTER;
    } else {
        DisplayInstructions("Transaction was not accepted by server. Press <strong>enter</strong> to continue.");
        state = STATE_WAIT_FOR_ENTER;
    }
}

/*
function DisplayServiceCost(data) {
    DisplayInstructions(`Service cost $${serviceCost}. Press <strong>enter</strong> to continue.`);
    state = STATE_WAIT_FOR_ENTER;
}
*/

function ResetStateForNextTransaction() {
    DisplayInstructions("Enter member number.");
    state = STATE_INPUT_MEMBER_ID;
}

function DecodeJsonResponse(response) {

    console.log(response.status);
    DisplayResponseStatus(response);

    if (response.ok) {
        return response.json();
    }
    return null;
}

function DisplayInstructions(instructions) {
    document.getElementById('instructions').innerHTML = instructions;
}

function DisplayResponseStatus(response) {
    document.getElementById('json-responses').innerHTML += `(${response.status})`;
}

function DisplayJson(json) {
    document.getElementById('json-responses').innerHTML += ` -- ${JSON.stringify(json)}\n`;
}

function HandleError(error) {
    console.log(`error: ${error}`);
}

function PrintHeader(response) {
    console.log(response.status);
    for (let [name, value] of response.headers) {
        console.log(`----${name}: ${value}\n`);
    }
}

