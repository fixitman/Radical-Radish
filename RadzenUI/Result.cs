﻿namespace RadzenUI
{	
	public class Result
	{
		private static readonly string EMPTY = "EMPTY";
		protected Result(bool success, string error)
		{
			if (success && error != string.Empty)
				throw new InvalidOperationException();
			if (!success && error == string.Empty)
				throw new InvalidOperationException();
			Success = success;
			Error = error;
		}

		public bool Success { get; }
		public string Error { get; }
		public bool IsFailure => !Success && Error != EMPTY;
		public bool IsEmpty => !Success && Error == EMPTY;
		public bool IsFailureOrEmpty => !Success;

		public static Result Fail(string message)
		{
			return new Result(false, message);
		}

		public static Result<T> Fail<T>(string message)
		{
			return new Result<T>(default, false, message);
		}

		public static Result Ok()
		{
			return new Result(true, string.Empty);
		}

		public static Result<T> Ok<T>(T value)
		{
			return new Result<T>(value, true, string.Empty);
		}
		
		public static Result Empty()
		{
			return new Result( false, EMPTY);
		}

		public static Result<T> Empty<T>()
		{
			return new Result<T>(default, false, EMPTY);
		}
	}

	public class Result<T> : Result
	{
		protected internal Result(T value, bool success, string error)
			: base(success, error)
		{
			Value = value;
		}

		public T Value { get; set; }
	}
	
}
