// **********************************************************************************
// * Copyright (c) 2022 Robin Murray
// **********************************************************************************
// *
// * File: terminal.js
// *
// * Description: Implements the provider's terminal state machine
// *
// **********************************************************************************
// * Author: Robin Murray
// **********************************************************************************
// *
// * Granting License: The MIT License (MIT)
// * 
// *   Permission is hereby granted, free of charge, to any person obtaining a copy
// *   of this software and associated documentation files (the "Software"), to deal
// *   in the Software without restriction, including without limitation the rights
// *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// *   copies of the Software, and to permit persons to whom the Software is
// *   furnished to do so, subject to the following conditions:
// *   The above copyright notice and this permission notice shall be included in
// *   all copies or substantial portions of the Software.
// *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// *   THE SOFTWARE.
// * 
// **********************************************************************************using System;

import ChocAnApi from "./chocanapi.js";

const api = new ChocAnApi();

// Terminal states
const STATE_INPUT_PROVIDER = 0;
const STATE_INPUT_MEMBER = 1;
const STATE_INPUT_PRODUCT = 2;
const STATE_INPUT_SERVICE_DATE = 3;
const STATE_ACCEPT_OR_REJECT_PRODUCT = 4;
const STATE_ENTER_COMMENT_AND_SUBMIT_TRANSACTION = 5;
const STATE_WAIT_FOR_ENTER = 6;

// Transaction fields
let providerId;
let providerName;
let memberId;
let memberStatus;
let productId;
let productName;
let productCost;
let serviceDate;
let serviceComment;

// Set first state
let state = STATE_INPUT_PROVIDER;
let dateEntryTimeout;

/// <summary>
/// Initializes page by:
/// - Adding eventlistener to input-form element for form submission
/// - Adding eventlistener to api url setting element and setting  default value
/// </summary>
(function () {

    // Attach an event listener to handle input from the form
    document
        .getElementById('input-form')
        .addEventListener("submit", event => onSubmit(event));

    // Attach an event listener for url setting and set value
    const baseUrlElement = document.getElementById('api-url');
    baseUrlElement.addEventListener("keyup", event => onApiUrlChange(event));
    baseUrlElement.value = api.url;

})();

/// <summary>
/// Event handler for API URL setting
/// </summary>
function onApiUrlChange(event) {
    api.url = document.getElementById('api-url').value;
}

/// <summary>
/// Event handler for terminal entries.  This function executes the terminal's
/// state machine state transitions.
/// </summary>
function onSubmit(event) {

    // Prevent default submit operation
    event.preventDefault();

    // Get trimmed text from terminal's input element
    let input = document.getElementById('input-element').value || "";
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

        case STATE_INPUT_PROVIDER: // submit provider ID
            FetchProvider(input);
            break;

        case STATE_INPUT_MEMBER:
            FetchMember(input);
            break;

        case STATE_INPUT_SERVICE_DATE:
            ClearDateEntryTimeout();
            EnterServiceDate(input);
            break;

        case STATE_INPUT_PRODUCT:
            FetchProduct(input);
            break;

        case STATE_ACCEPT_OR_REJECT_PRODUCT:
            AcceptOrRejectServiceCode(input);
            break;

        case STATE_ENTER_COMMENT_AND_SUBMIT_TRANSACTION:
            EnterServiceComment(input);
            PostTransaction();
            break;

        case STATE_WAIT_FOR_ENTER:
            ResetToInputMemberIdState();
            break;
    }

    // Clear the terminal's input element
    ClearInputline();
}

/// <summary>
/// Clears the terminal's input element
/// </summary>
function ClearInputline() {
    document.getElementById('input-element').value = "";
}

/// <summary>
/// Starts the timeout for entering the transaction's date
/// </summary>
function startDateEntryTimeout() {
    dateEntryTimeout = setTimeout(HandleDateEntryTimeout, 60000);
}

/// <summary>
/// Clears the timeout for entering the transaction's date
/// </summary>
function ClearDateEntryTimeout() {
    clearTimeout(dateEntryTimeout);
}

/// <summary>
/// Handles the timeout event for entering the transaction date by clearing
/// the input box and resetting the state machine to it's initial state
/// </summary>
function HandleDateEntryTimeout() {
    ResetToInputMemberIdState();
    ClearInputline();
}

/// <summary>
/// Fetches the provider's ID from the ChocAn API and delegates the response
/// </summary>
function FetchProvider(id) {

    fetch(api.provider(id))
        .then(response => DecodeJsonResponse(response))
        .then(data => HandleProviderResponse(data))
        .catch(error => HandleError(error));
}

/// <summary>
/// Processes the response from the api/terminal/provider/id endpoint.  If valid
/// data is returned the provider's ID and name are recorded for the
/// subsequent transaction and the state machine advances to the STATE_INPUT_MEMBER
/// state.  If provider was not found the state machine advances to the STATE_INPUT_PROVIDER.
/// </summary>
function HandleProviderResponse(data) {

    DisplayJson(data);
    if (data) {
        providerId = data.id;
        providerName = data.name;
        DisplayInstructions("Enter member number.");
        state = STATE_INPUT_MEMBER;
    } else {
        DisplayInstructions("Provider number not found, re-enter provider number.");
        state = STATE_INPUT_PROVIDER;
    }
}

/// <summary>
/// Fetches the member's ID from the ChocAn API and delegates the response
/// </summary>
function FetchMember(id) {

    fetch(api.member(id))
        .then(response => DecodeJsonResponse(response))
        .then(data => HandleMemberResponse(data))
        .catch(error => HandleError(error));
}

/// <summary>
/// Processes the response from the api/terminal/member/id endpoint.  If the
/// member is not active or not found it is displayed to the user and the 
/// state machine advances to the STATE_WAIT_FOR_ENTER.  If the member is active
/// the state machine advances to the STATE_INPUT_SERVICE_DATE.
/// </summary>
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
            startDateEntryTimeout();
            state = STATE_INPUT_SERVICE_DATE;
        }
    } else {
        DisplayInstructions("Member number not found. Press <strong>enter</strong> to continue.");
        state = STATE_WAIT_FOR_ENTER;
    }
}

/// <summary>
/// Translates the input parameter into an ISO date for the subsequent 
/// transaction, then advances the state machine to STATE_INPUT_PRODUCT.
/// </summary>
function EnterServiceDate(input) {

    serviceDate = (new Date(input)).toISOString();

    console.log(`Service Date: ${serviceDate}`)

    DisplayInstructions("Enter product code.");
    state = STATE_INPUT_PRODUCT;
}

/// <summary>
/// Fetches the service's ID from the ChocAn API and delegates the response
/// </summary>
function FetchProduct(id) {

    fetch(api.product(id))
        .then(response => DecodeJsonResponse(response))
        .then(data => HandelProductResponse(data))
        .catch(error => HandleError(error));
}

/// <summary>
/// Processes the response from the api/terminal/service/id endpoint.  If valid
/// data is returned the service's ID, name, and cost are recorded for the
/// subsequent transaction and the state machine advances to the STATE_ACCEPT_OR_REJECT_PRODUCT
/// state.  If the service was not found the state machine advances to the STATE_INPUT_PRODUCT.
/// </summary>
function HandelProductResponse(data) {

    DisplayJson(data);
    if (data) {
        productId = data.id;
        productName = data.name;
        productCost = data.cost;
        DisplayInstructions(`<strong>${productName}</strong>: enter <strong>Yes</strong> to accept product code or <strong>No</strong> to re-enter product code`);
        state = STATE_ACCEPT_OR_REJECT_PRODUCT;
    } else {
        DisplayInstructions("Product code not found, re-enter product code.");
        state = STATE_INPUT_PRODUCT;
    }
    console.log(`Product code: ${productId}`);
}

/// <summary>
/// Processes the yes or no answer from the STATE_ACCEPT_OR_REJECT_PRODUCT state.
/// if "yes" or "y" is entered (case insensitive) the state machine advances to the 
/// STATE_ENTER_COMMENT_AND_SUBMIT_TRANSACTION state
/// if "no" or "n" is entered (case insensitive) the state machine advances to the 
/// STATE_INPUT_PRODUCT state
/// Any other answer is rejected and the state advances to STATE_ACCEPT_OR_REJECT_PRODUCT
/// </summary>
function AcceptOrRejectServiceCode(text) {

    let answer = text.toLowerCase();

    if ((answer == "yes") || (answer == "y")) {
        DisplayInstructions("Enter comment about service or just press enter for no comment");
        state = STATE_ENTER_COMMENT_AND_SUBMIT_TRANSACTION;
    } else if ((answer == "no") || (answer == "n")) {
        DisplayInstructions("Re-enter product code...");
        state = STATE_INPUT_PRODUCT;
    } else {
        DisplayInstructions(`Did not understand answer. <strong>${productName}</strong>: enter <strong>Yes</strong> to accept product code or <strong>No</strong> to re-enter product code`);
        state = STATE_ACCEPT_OR_REJECT_PRODUCT;
    }
}

/// <summary>
/// Records service comment for later incorporation into service transaction
/// </summary>
function EnterServiceComment(text) {
    serviceComment = text;
}

/// <summary>
/// Posts the transaction to the ChocAn transaction API
/// </summary>
function PostTransaction() {

    let transaction = {};
    transaction.providerId = providerId;
    transaction.memberId = memberId;
    transaction.productId = productId;
    transaction.productCost = productCost
    transaction.serviceDate = serviceDate;
    transaction.serviceComment = serviceComment;

    console.log(`Post Transaction:`)
    console.log(transaction)

    fetch(api.transaction, {
        method: "POST",
        headers: new Headers({ "content-type": "application/json" }),
        body: JSON.stringify(transaction)
    })
        .then(response => DecodeJsonResponse(response))
        .then(data => HandleTransactionResponse(data))
        .catch(error => HandleError(error));
}

/// <summary>
/// Processes the response from the api/terminal/transaction endpoint.  If the
/// transaction was accepted the user is told so and the state advances to
/// STATE_WAIT_FOR_ENTER.  If the transaction was not accepted the user is told 
/// so and the state advances to STATE_WAIT_FOR_ENTER
/// </summary>
function HandleTransactionResponse(data) {

    DisplayJson(data);

    if (data) {
        DisplayInstructions(`Transaction accepted: Service cost $${productCost}. Press <strong>enter</strong> to continue.`);
        state = STATE_WAIT_FOR_ENTER;
    } else {
        DisplayInstructions("Transaction was not accepted by server. Press <strong>enter</strong> to continue.");
        state = STATE_WAIT_FOR_ENTER;
    }
}

/// <summary>
/// Displays instructions to enter member's ID and then advances state machine to
/// STATE_INPUT_MEMBER
/// </summary>
function ResetToInputMemberIdState() {
    DisplayInstructions("Enter member number.");
    state = STATE_INPUT_MEMBER;
}

/// <summary>
/// 1) Appends JSON response to responses text box.  
/// 2) If response status was OK(i.e. 200) then the JSON data is returned for 
///    processing by the next handler.  Otherwise null is returned.
/// </summary>
function DecodeJsonResponse(response) {

    console.log(response.status);
    DisplayResponseStatus(response);

    if (response.ok) {
        return response.json();
    }
    return null;
}

/// <summary>
/// Displays instructions in the instructions element
/// </summary>
function DisplayInstructions(instructions) {
    document.getElementById('instructions').innerHTML = instructions;
}

/// <summary>
/// Appends JSON responses to the JSON response textbox
/// </summary>
function DisplayResponseStatus(response) {
    document.getElementById('json-responses').innerHTML += `(${response.status})`;
}

/// <summary>
/// Appends the stringified version of the JSON responses to the JSON response textbox
/// </summary>
function DisplayJson(json) {
    document.getElementById('json-responses').innerHTML += ` -- ${JSON.stringify(json)}\n`;
}

/// <summary>
/// Logs error to the console
/// </summary>
function HandleError(error) {
    console.log(`error: ${error}`);
}

/// <summary>
/// Logs response headers to the console
/// </summary>
function PrintHeader(response) {
    console.log(response.status);
    for (let [name, value] of response.headers) {
        console.log(`----${name}: ${value}\n`);
    }
}
