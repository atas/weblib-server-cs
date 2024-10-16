﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebLibServer.Videos;

namespace WebLibServer.RequestModels;

public class ScraperApiAddPostReqModel
{
	public int UserId { get; set; }

	[Required]
	public string PostTitle { get; set; }

	public List<string> LabelNames { get; set; }

	public string ScrapeRefUrl { get; set; }

	[Required]
	public VideoWithPreviewUploader.Result UploadResult { get; set; }
}
