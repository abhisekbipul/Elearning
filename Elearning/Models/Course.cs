﻿using System;
using System.Collections.Generic;

namespace Elearning.Models;

public partial class Course
{
    public int Id { get; set; }

    public string? Cname { get; set; }

    public string? Subname { get; set; }

    public decimal? Price { get; set; }
}
