// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: Member.cs
// *
// * Description: Defines an entity which describes a ChocAn member
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
// **********************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using ChocAn.Repository.Sorting;
using ChocAn.Repository.Search;

namespace ChocAn.Data
{
    /// <summary>
    /// Represents a ChocAn member
    /// </summary>
    public class Member
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Sortable(Default = true)]
        [Searchable]
        public string Name { get; set; }
        [Searchable]
        public string Email { get; set; }
        public string StreetAddress { get; set; }
        [Sortable]
        [Searchable]
        public string City { get; set; }
        [Sortable]
        [Searchable]
        public string State { get; set; }
        [Sortable]
        [SearchableInt]
        public int ZipCode { get; set; }
        [Sortable]
        [Searchable]
        public string Status { get; set; }
    }
}
