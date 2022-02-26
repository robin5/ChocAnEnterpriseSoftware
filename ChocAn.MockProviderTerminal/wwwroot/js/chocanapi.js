// **********************************************************************************
// * Copyright (c) 2022 Robin Murray
// **********************************************************************************
// *
// * File: chocanapi.js.cs
// *
// * Description: Thsi class defines URLs to the ChocAn API
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

// Key used for storing ChocAn API's base URL in local storage
const keyApiUrl = 'api-url';

export default class ChocAnApi {

    /// <summary>
    /// Loads ChocAn API's base URL from local storage
    /// </summary>
    constructor() {
        this.url_ = localStorage.getItem(keyApiUrl) || '';
        console.log(`constructor: ${this.url_}`);
    }

    /// <summary>
    /// Returns base ChocAn API URL
    /// </summary>
    get url() {
        return this.url_;
    }

    /// <summary>
    /// Sets base ChocAn API URL assuring to add a "/" to the end of it and
    /// saving it to local storage
    /// </summary>
    set url(url) {
        if (!url.endsWith('/'))
            url += '/';
        this.url_ = url;
        localStorage.setItem(keyApiUrl, url);
    }

    /// <summary>
    /// Returns a URL representing the {chocan.domain/api/}terminal/provider/{id}
    /// </summary>
    provider = (id) => {
        return `${this.url_}terminal/provider/${id}`;
    }

    /// <summary>
    /// Returns a URL representing the {chocan.domain/api/}terminal/member/{id}
    /// </summary>
    member = (id) => {
        return `${this.url_}terminal/member/${id}`;
    }

    /// <summary>
    /// Returns a URL representing the {chocan.domain/api/}terminal/service/{id}
    /// </summary>
    service = (id) => {
        return `${this.url_}terminal/service/${id}`;
    }

    /// <summary>
    /// Returns a URL representing the {chocan.domain/api/}terminal/transaction
    /// </summary>
    get transaction () {
        return `${this.url_}terminal/transaction/`;
    }
}
