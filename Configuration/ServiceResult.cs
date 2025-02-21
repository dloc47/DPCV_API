namespace DPCV_API.Configuration
{
    /// <summary>
    /// Standardized response model for API service results.
    /// Ensures consistency in API responses by providing structured error handling and metadata.
    /// </summary>
    public class ServiceResult
    {
        /// <summary>
        /// A list of error messages encountered during the request.
        /// Set to `null` when no errors exist, reducing unnecessary data in responses.
        /// </summary>
        public List<string>? Errors { get; set; } = null;

        /// <summary>
        /// Indicates whether the request was successful.
        /// Returns `true` if `Errors` is null or contains no elements.
        /// </summary>
        public bool IsSuccessful => Errors == null || Errors.Count == 0;

        /// <summary>
        /// Additional metadata or extra data that may be required in the response.
        /// Can be used for sending additional debugging information, optional data, etc.
        /// </summary>
        public object? Extras { get; set; } = null;

        /// <summary>
        /// Unique reference ID for tracking API requests.
        /// Useful for logging and debugging to trace specific requests.
        /// </summary>
        public string? RefId { get; set; } = null;

        /// <summary>
        /// The main data returned by the API.
        /// Defaults to `null` when no relevant data is available.
        /// </summary>
        public object Data { get; set; } = new List<object>(); // Ensures empty list when no data exists

        /// <summary>
        /// A user-friendly message describing the outcome of the request.
        /// Defaults to an empty string but should be set to relevant messages.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the error is of a critical type.
        /// Can be used to differentiate between validation errors and system failures.
        /// </summary>
        public bool ErrorType { get; set; } = false;

        /// <summary>
        /// HTTP status code associated with the response.
        /// Helps in API consumers determining the nature of the response (e.g., `200`, `400`, `500`).
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Constructor to initialize the response object with a default message and status code.
        /// </summary>
        /// <param name="message">The message describing the outcome of the request.</param>
        /// <param name="statusCode">The HTTP status code for the response (default is `200`).</param>
        public ServiceResult(string message = "", int statusCode = 200)
        {
            Message = message;
            StatusCode = statusCode;
        }

        /// <summary>
        /// Adds a single error message to the `Errors` list.
        /// Initializes the list if it is currently null.
        /// </summary>
        /// <param name="errorMessage">The error message to be added.</param>
        public void AddError(string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                Errors ??= new List<string>(); // Initialize only if needed
                Errors.Add(errorMessage);
            }
        }

        /// <summary>
        /// Adds multiple error messages to the `Errors` list.
        /// Initializes the list if it is currently null.
        /// </summary>
        /// <param name="errorMessages">A list of error messages to add.</param>
        public void AddErrors(List<string> errorMessages)
        {
            if (errorMessages != null && errorMessages.Count > 0)
            {
                Errors ??= new List<string>(); // Initialize only if needed
                Errors.AddRange(errorMessages);
            }
        }
    }
}
