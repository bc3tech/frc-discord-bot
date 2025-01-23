/*
 * FRC Events
 *
 * # Overview  _FIRST_/FMS FRC Events API is a service to return relevant information about the _FIRST_ Robotics Competition (FRC). Information is made available from events operating around the world.  For FRC, information is made available by the Field Management System (FMS) server operating at each event site. The FMS will attempt to sync all data from the event to \"the cloud\" as long as internet is available at the venue. If internet is unavailable, or \"goes down\" during the event, the FMS will automatically sync all data from the time the system was offline as soon as the connection is restored. The API will provide data as soon as it has synced, and we do not add any artificial delays. If you are receiving \"stale\" data, it may be because of connectivity problems at the venue. We recommend you try again later, and post on the FIRST FMS TeamForge site if the problem persists. _(Note: FMS does not sync while a match is running, so data that has to do with a particular match should become available once the score has been revealed to the audience at the event.)_  ### Migration and Program Notes:  Pay close attention to the addresses for calling the various endpoints- as well as the notes regarding endpoints with multiple possible responses (i.e. score details and rankings).  # Documentation Notes  All times are listed in the local time to the event venue. HTTP-date values will show their timezone.  If you specify a parameter, but no value for that parameter, it will be ignored. For example, if you request **URL?teamNumber=** the **teamNumber** parameter would be ignored.  We will continue to support the current version of the API plus one version older. Old APIs are depricated once a version \"two times newer\" is available, at minimum 6 months. For example, version 2.0 and 1.0 are supported right now, but 1.0 will not be supported once 2.1 (or 3.0) is available. Versions may also be retired earlier with prior notice here in the documentation.  The full host address of the API is needed in all calls. The version number is required in each call in order to ensure your requests are made (and responses are returned) in the formats you anticipate. The version number for this documentation is found on the top of the page, in the title. If you call this version number, the API responses will match the formats listed below.  All of the APIs are capable of accepting the **Accept** HTTP header with either **application/xml** or **application/json** based on the desired return type. Any API call that results in an **HTTP Status Code** other than **200 (OK)** will only be shown here as an **application/json** response to save space, but the content is the same regardless of the request type. All response will have a **Content-Length** and **Date** response header, but those are not shown in the documentaiton.  For all APIs that accept a query string in addition to the URI base, the order of parameters do not matter, but the name shown in the documentation must match exactly, as does the associated value format as described in details.  For response codes that are not **HTTP 200 (OK)**, the documentation will show a body message that represents a possible response value. While the \"title\" of the **HTTP Status Code** will match those shown in the response codes documentation section exactly, the body of the response will be a more detailed explanation of why that status code is being returned and may not always be exactly as shown in the examples.  None of the APIs will show possible return here in the documentation of **HTTP 401 (Unauthorized)**, but that code applies to all APIs as a possible response if the request is made without a valid token.  ### Last-Modified, FMS-OnlyModifiedSince, and If-Modified-Since Headers  The FRC Events API utilizes the **Last-Modified** and **If-Modified-Since** Headers to communicate with consumers regarding the age of the data they are requesting. With a couple of exceptions, all calls will return a **Last-Modified** Header set with the time at which the data at that endpoint was last modified. The Header will always be set in the HTTP-date format, as described in the [HTTP Protocol](https://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html). There are two exceptions: the **Last-Modified** Header is not set if the endpoint returns no results (such as a request for a schedule with no matches) and will also not be set if the request was an **HTTP DELETE**.  Consumers should keep track of the **Last-Modified** Header, and return it on subsequent calls to the same endpoint as the **If-Modified-Since**. The server will recognize this request, and will only return a result if the data has been modified since the last request. If no changes have been made, an **HTTP 304** will be returned. If data has been modified, ALL data on that call will be returned (for \"only modified\" data, see below).  The FRC Events API also allows a custom header used to filter the return data to a specific subset. This is done by specifying a **FMS-OnlyModifiedSince** header with each call. As with the **If-Modified-Since** header, consumers should keep track of the **Last-Modified** Header, and return it on subsequent calls to the same endpoint as the **FMS-OnlyModifiedSince** Header. The server will recognize this request, and will only return a result if the data has been modified since the last request, and, if returned, the data will only be those portions modified since the included date. If no changes, have been made, an **HTTP 304** will be returned. Using this method, the server and consumer save processing time by only receiving modified data that is in need of update on the consumer side.  If the Headers are improperly passed (such as the wrong Day of Week for the matching date, or a date in the future), the endpoint will simply ignore the Header and return all results. If both headers are specified, the request will be denied.  # Response Codes  The FRC Events API HTTP Status Codes correspond with the [common codes](https://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html), but occasionally with different \"titles\". The \"title\" used by the API is shown next to each of the below possible response HTTP Status Codes. Throughout the documentation, Apiary may automatically show the common \"title\" in example returns (like \"Not Found\" for 404) but on the production server, the \"title\" will instead match those listed below.  ``` HTTP 200 - \"OK\"   ```  The request has succeeded. An entity corresponding to the requested resource is sent in the response. This will be returned as the HTTP Status Code for all request that succeed, even if the body is empty (such as an event that has no rankings, but with a valid season and event code were used)  ``` HTTP 304 - \"Not Modified\"   ```  When utilizing a Header that allows filtered data returns, such as **If-Modified-Since**, this response indicates that no data meets the request.  ``` HTTP 400 - \"Invalid Season Requested\"/\"Malformed Parameter Format In Request\"/\"Missing Parameter In Request\"/\"Invalid API Version Requested\":   ```  The request could not be understood by the server due to malformed syntax. The client SHOULD NOT repeat the request without modifications. Specifically for this API, a 400 response indicates that the requested URI matches with a valid API, but one or more required parameter was malformed or invalid. Examples include an event code that is too short or team number that contains a letter.  ``` HTTP 401 - \"Unauthorized\"   ```  All requests against the API require authentication via a valid user token. Failing to provide one, or providing an invalid one, will warrant a 401 response. The client MAY repeat the request with a suitable Authorization header field.  ``` HTTP 404 - \"Invalid Event Requested\"   ```  Even though the 404 code usually indicates any not found status, a 404 will only be issued in this API when an event cannot be found for the requested season and event code. If the request didn't match a valid API or there were malformed parameters, the response would not receive a 404 but rather a 400 or 501. If this HTTP code is received, the season was a valid season and the event code matched the acceptable style of an event code, but there were no records of an event matching the combination of that season and event code. For example, HTTP 404 would be issued when the event had a different code in the requested season (the codes can change year to year based on event location).  ``` HTTP 500 - \"Internal Server Error\"   ```  The server encountered an unexpected condition which prevented it from fulfilling the request. This is a code sent directly by the server, and has no special alternate definition specific to this API.  ``` HTTP 501 - \"Request Did Not Match Any Current API Pattern\"   ```  The server does not support the functionality required to fulfill the request. Specifically, the request pattern did not match any of the possible APIs, and thus processing was discontinued. This code is also issued when too many optional parameters were included in a single request and fulfilling it would make the result confusing or misleading. Each API will specify which parameters or combination of parameters can be used at the same time.  ``` HTTP 503 - \"Service Unavailable\"   ```  The server is currently unable to handle the request due to a temporary overloading or maintenance of the server. The implication is that this is a temporary condition which will be alleviated after some delay. If known, the length of the delay MAY be indicated in a Retry-After header. This code will not always appear, sometimes the server may outright refuse the connection instead. This is a code sent directly by the server, and has no special alternate definition specific to this API.  See the notes at the top of this documentation for important information about HTTP Status Codes.  # Authorization  In order to make calls against the FRC Events API, you must include an HTTP Header called **Authorization** with the value set as specified below. If a request is made without this header, processing stops and an **HTTP 401** is issued. All **Authorization** headers follow the same format:  Authorization: Basic 000000000000000000000000000000000000000000000000000000000000  Where the Zeros are replaced by your Token. The Token can be formed by taking your **username** and your **AuthorizationKey** and adding a colon. For example, if your **username** is sampleuser and your **AuthorizationKey** is 7eaa6338-a097-4221-ac04-b6120fcc4d49 you would have this string:  **sampleuser:7eaa6338-a097-4221-ac04-b6120fcc4d49**  This string must then be encoded using Base64 Encoded to form the Token, which will be the same length as the example above, but include letters and numbers. For our example, we would have:  c2FtcGxldXNlcjo3ZWFhNjMzOC1hMDk3LTQyMjEtYWMwNC1iNjEyMGZjYzRkNDk=  **NOTICE**: Publicly distributing an application, code snippet, etc, that has your username and token in it, encoded or not, WILL result in your token being blocked from the API. Each user should apply for their own token.  If you wish to acquire a token for your development, you may do so by requesting a token through our automated system on [this website](https://frc-events.firstinspires.org/services/API).  **AUTOMATED REMOVAL**: If you do not activate your account within 72 hours of making your request for a token, or if you do not make at least one API request every twelve (12) months, your account/token will be marked as disabled for inactivity and subject to being deleted. (This policy does not apply to accounts with special operating agreements with FIRST)  ### HTTP401 and Authorization  Each Token can be individually enabled and disabled by _FIRST_. As such, a normally valid combination of **username** and **AuthorizationToken** could still be rejected. The possible return messages you may see in these instances are:  **Incorrect Token** (You supplied an AuthorizationToken, but it wasn't correct)  **Account Disabled, Contact Support** (You have been disabled for excessive traffic or abuse. Contact support)  **Username Not Found** (A username was found, but didn't match any on file)  **Unable To Determine Authorization Token** (The format of the **Authorization** header was incorrect)  # Webhooks  > **COMING SOON**
 *
 * The version of the OpenAPI document: 1.0.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace FIRST.Client;

using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A dictionary in which one key has many associated values.
/// </summary>
/// <typeparam name="TKey">The type of the key</typeparam>
/// <typeparam name="TValue">The type of the value associated with the key.</typeparam>
public class Multimap<TKey, TValue> : IDictionary<TKey, IList<TValue>>
{
  #region Private Fields
  
  private readonly Dictionary<TKey, IList<TValue>> _dictionary;
  
  #endregion Private Fields
  
  #region Constructors
  
  /// <summary>
  /// Empty Constructor.
  /// </summary>
  public Multimap()
  {
    _dictionary = new Dictionary<TKey, IList<TValue>>();
  }
  
  /// <summary>
  /// Constructor with comparer.
  /// </summary>
  /// <param name="comparer"></param>
  public Multimap(IEqualityComparer<TKey> comparer)
  {
    _dictionary = new Dictionary<TKey, IList<TValue>>(comparer);
  }
  
  #endregion Constructors
  
  #region Enumerators
  
  /// <summary>
  /// To get the enumerator.
  /// </summary>
  /// <returns>Enumerator</returns>
  public IEnumerator<KeyValuePair<TKey, IList<TValue>>> GetEnumerator()
  {
    return _dictionary.GetEnumerator();
  }
  
  /// <summary>
  /// To get the enumerator.
  /// </summary>
  /// <returns>Enumerator</returns>
  IEnumerator IEnumerable.GetEnumerator()
  {
    return _dictionary.GetEnumerator();
  }
  
  #endregion Enumerators
  
  #region Public Members
  /// <summary>
  /// Add values to Multimap
  /// </summary>
  /// <param name="item">Key value pair</param>
  public void Add(KeyValuePair<TKey, IList<TValue>> item)
  {
    if (!TryAdd(item.Key, item.Value))
    throw new InvalidOperationException("Could not add values to Multimap.");
  }
  
  /// <summary>
  /// Add Multimap to Multimap
  /// </summary>
  /// <param name="multimap">Multimap</param>
  public void Add(Multimap<TKey, TValue> multimap)
  {
    foreach (var item in multimap)
    {
      if (!TryAdd(item.Key, item.Value))
      throw new InvalidOperationException("Could not add values to Multimap.");
    }
  }
  
  /// <summary>
  /// Clear Multimap
  /// </summary>
  public void Clear()
  {
    _dictionary.Clear();
  }
  
  /// <summary>
  /// Determines whether Multimap contains the specified item.
  /// </summary>
  /// <param name="item">Key value pair</param>
  /// <exception cref="NotImplementedException">Method needs to be implemented</exception>
  /// <returns>true if the Multimap contains the item; otherwise, false.</returns>
  public bool Contains(KeyValuePair<TKey, IList<TValue>> item)
  {
    throw new NotImplementedException();
  }
  
  /// <summary>
  ///  Copy items of the Multimap to an array,
  ///     starting at a particular array index.
  /// </summary>
  /// <param name="array">The array that is the destination of the items copied
  ///     from Multimap. The array must have zero-based indexing.</param>
  /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
  /// <exception cref="NotImplementedException">Method needs to be implemented</exception>
  public void CopyTo(KeyValuePair<TKey, IList<TValue>>[] array, int arrayIndex)
  {
    throw new NotImplementedException();
  }
  
  /// <summary>
  /// Removes the specified item from the Multimap.
  /// </summary>
  /// <param name="item">Key value pair</param>
  /// <returns>true if the item is successfully removed; otherwise, false.</returns>
  /// <exception cref="NotImplementedException">Method needs to be implemented</exception>
  public bool Remove(KeyValuePair<TKey, IList<TValue>> item)
  {
    throw new NotImplementedException();
  }
  
  /// <summary>
  /// Gets the number of items contained in the Multimap.
  /// </summary>
  public int Count => _dictionary.Count;
  
  /// <summary>
  /// Gets a value indicating whether the Multimap is read-only.
  /// </summary>
  public bool IsReadOnly => false;
  
  /// <summary>
  /// Adds an item with the provided key and value to the Multimap.
  /// </summary>
  /// <param name="key">The object to use as the key of the item to add.</param>
  /// <param name="value">The object to use as the value of the item to add.</param>
  /// <exception cref="InvalidOperationException">Thrown when couldn't add the value to Multimap.</exception>
  public void Add(TKey key, IList<TValue> value)
  {
    if (value is not null && value.Count > 0)
    {
      if (_dictionary.TryGetValue(key, out var list))
      {
        foreach (var k in value) list.Add(k);
      }
      else
      {
        list = new List<TValue>(value);
        if (!TryAdd(key, list))
        throw new InvalidOperationException("Could not add values to Multimap.");
      }
    }
  }
  
  /// <summary>
  /// Determines whether the Multimap contains an item with the specified key.
  /// </summary>
  /// <param name="key">The key to locate in the Multimap.</param>
  /// <returns>true if the Multimap contains an item with
  ///     the key; otherwise, false.</returns>
  public bool ContainsKey(TKey key)
  {
    return _dictionary.ContainsKey(key);
  }
  
  /// <summary>
  /// Removes item with the specified key from the Multimap.
  /// </summary>
  /// <param name="key">The key to locate in the Multimap.</param>
  /// <returns>true if the item is successfully removed; otherwise, false.</returns>
  public bool Remove(TKey key)
  {
    return TryRemove(key, out var _);
  }
  
  /// <summary>
  /// Gets the value associated with the specified key.
  /// </summary>
  /// <param name="key">The key whose value to get.</param>
  /// <param name="value">When this method returns, the value associated with the specified key, if the
  ///     key is found; otherwise, the default value for the type of the value parameter.
  ///     This parameter is passed uninitialized.</param>
  /// <returns> true if the object that implements Multimap contains
  ///     an item with the specified key; otherwise, false.</returns>
  public bool TryGetValue(TKey key, out IList<TValue> value)
  {
    return _dictionary.TryGetValue(key, out value);
  }
  
  /// <summary>
  /// Gets or sets the item with the specified key.
  /// </summary>
  /// <param name="key">The key of the item to get or set.</param>
  /// <returns>The value of the specified key.</returns>
  public IList<TValue> this[TKey key]
  {
    get => _dictionary[key];
    set => _dictionary[key] = value;
  }
  
  /// <summary>
  /// Gets a System.Collections.Generic.ICollection containing the keys of the Multimap.
  /// </summary>
  public ICollection<TKey> Keys => _dictionary.Keys;
  
  /// <summary>
  /// Gets a System.Collections.Generic.ICollection containing the values of the Multimap.
  /// </summary>
  public ICollection<IList<TValue>> Values => _dictionary.Values;
  
  /// <summary>
  ///  Copy the items of the Multimap to an System.Array,
  ///     starting at a particular System.Array index.
  /// </summary>
  /// <param name="array">The one-dimensional System.Array that is the destination of the items copied
  ///     from Multimap. The System.Array must have zero-based indexing.</param>
  /// <param name="index">The zero-based index in array at which copying begins.</param>
  public void CopyTo(Array array, int index)
  {
    ((ICollection)_dictionary).CopyTo(array, index);
  }
  
  /// <summary>
  /// Adds an item with the provided key and value to the Multimap.
  /// </summary>
  /// <param name="key">The object to use as the key of the item to add.</param>
  /// <param name="value">The object to use as the value of the item to add.</param>
  /// <exception cref="InvalidOperationException">Thrown when couldn't add value to Multimap.</exception>
  public void Add(TKey key, TValue value)
  {
    if (value is not null)
    {
      if (_dictionary.TryGetValue(key, out var list))
      {
        list.Add(value);
      }
      else
      {
        list = new List<TValue> { value };
        if (!TryAdd(key, list))
        throw new InvalidOperationException("Could not add value to Multimap.");
      }
    }
  }
  
  #endregion Public Members
  
  #region Private Members
  
  /**
  * Helper method to encapsulate generator differences between dictionary types.
  */
  private bool TryRemove(TKey key, out IList<TValue> value)
  {
    _dictionary.TryGetValue(key, out value);
    return _dictionary.Remove(key);
  }
  
  /**
  * Helper method to encapsulate generator differences between dictionary types.
  */
  private bool TryAdd(TKey key, IList<TValue> value)
  {
    try
    {
      _dictionary.Add(key, value);
    }
    catch (ArgumentException)
    {
      return false;
    }
    
    return true;
  }
  #endregion Private Members
}