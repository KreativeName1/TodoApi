# TodoNotes API Documentation

### TodoNoteController

This controller manages Todo Notes for authenticated users. All endpoints require authorization.

#### GetTodoNote (GET /TodoNote/{id})

* **Info:** Retrieves a specific TodoNote by its ID.
* **Requirements:**
    * User must be authenticated.
    * User must be authorized to view the TodoNote (ownership check enforced).
* **Request Body:**
   ```
   No request body required.
   ```
* **Response:**
    * **200 (OK):** Returns the requested TodoNote object.
    * **401 (Unauthorized):** If the user is not authenticated.
    * **404 (Not Found):** If the TodoNote with the specified ID is not found.
    * **403 (Forbidden):** If the user is not authorized to view the TodoNote.

* **Example (curl):**

```
# Assuming you have a JWT access token stored in a variable named 'ACCESS_TOKEN'
curl -H "Authorization: Bearer $ACCESS_TOKEN" http://localhost:5000/TodoNote/1
```

#### CreateTodoNote (POST /TodoNote)

* **Info:** Creates a new TodoNote for the authenticated user.
* **Requirements:**  
    * User must be authenticated.
* **Request Body:**
   ```json
  {
    "Title": "string", (required)
    "Content": "string", (required)
    "DueDate": "datetime", (optional)
  }
  ```
* **Response:**
    * **201 (Created):** Returns the newly created TodoNote object.
    * **401 (Unauthorized):** If the user is not authenticated.
    * **400 (Bad Request):** If the request body is invalid or missing required fields.

* **Example (curl):**

```
curl -X POST -H "Authorization: Bearer $ACCESS_TOKEN" -H "Content-Type: application/json" -d '{ "Title": "Buy Groceries", "Content": "Milk, Bread, Eggs" }' http://localhost:5000/TodoNote
```

#### UpdateTodoNote (PUT /TodoNote/{id})
* __Not tested yet__
* **Info:** Updates an existing TodoNote.
* **Requirements:**  
    * User must be authenticated.
    * User must be authorized to update the TodoNote (ownership check enforced).
* **Request Body:**
   ```json
  {
    "Title": "string", (optional)
    "Content": "string", (optional)
    "DueDate": "datetime", (optional)
    "IsComplete": "boolean", (optional)
  }
  ```
* **Response:**
    * **200 (OK):** Returns the updated TodoNote object.
    * **401 (Unauthorized):** If the user is not authenticated.
    * **404 (Not Found):** If the TodoNote with the specified ID is not found.
    * **403 (Forbidden):** If the user is not authorized to update the TodoNote.

* **Example (curl):**

```
curl -X PUT -H "Authorization: Bearer $ACCESS_TOKEN" -H "Content-Type: application/json" -d '{ "Title": "Updated Title", "Content": "New Content" }' http://localhost:5000/TodoNote/1
```

#### DeleteTodoNote (DELETE /TodoNote/{id})
* __Not tested yet__
* **Info:** Deletes a TodoNote.
* **Requirements:**
    * User must be authenticated.
    * User must be authorized to delete the TodoNote (ownership check enforced).
* **Request Body:**
   ```json
    No request body required.
   ```
* **Response:**
    * **200 (OK):** Returns the deleted TodoNote object.
    * **401 (Unauthorized):** If the user is not authenticated.
    * **404 (Not Found):** If the TodoNote with the specified ID is not found.
    * **403 (Forbidden):** If the user is not authorized to delete the TodoNote.

* **Example (curl):**

```
curl -X DELETE -H "Authorization: Bearer $ACCESS_TOKEN" http://localhost:5000/TodoNote/1
```
#### GetTodoNotes (GET /TodoNote/user/)

* **Info:** Retrieves all TodoNotes for the authenticated user.
* **Requirements:**  
    * User must be authenticated.
* **Request Body:**
   ```json
   No request body required.
   ```
* **Response:**
    * **200 (OK):** Returns a list of TodoNote objects for the authenticated user.
    * **401 (Unauthorized):** If the user is not authenticated.

* **Example (curl):**

```
# Assuming you have a JWT access token stored in a variable named 'ACCESS_TOKEN'
curl -H "Authorization: Bearer $ACCESS_TOKEN" http://localhost:5000/TodoNote/user/
```

#### MarkCompleteTodoNote (PUT /TodoNote/{id}/markComplete)

* **Info:** Marks a specific TodoNote as completed.
* **Requirements:**  
    * User must be authenticated.
    * User must be authorized to update the TodoNote (ownership check enforced).
* **Request Body:**
   ```json
   No request body required.
   ```
* **Response:**
    * **200 (OK):** Returns the updated TodoNote object with the IsComplete flag set to true.
    * **401 (Unauthorized):** If the user is not authenticated.
    * **404 (Not Found):** If the TodoNote with the specified ID is not found.
    * **403 (Forbidden):** If the user is not authorized to update the TodoNote.

* **Example (curl):**

```
# Assuming you have a JWT access token stored in a variable named 'ACCESS_TOKEN'
curl -X PUT -H "Authorization: Bearer $ACCESS_TOKEN" http://localhost:5000/TodoNote/1/markComplete
```


### UserController

This controller manages user information.  All endpoints require authorization.

#### GetUser (GET /User)

* **Info:** Retrieves information for the currently authenticated user.
* **Requirements:**  
    * User must be authenticated.
* **Request Body:**
   ```json
    No request body required.
   ```
* **Response:**
    * **200 (OK):** Returns the User object for the authenticated user.
    * **401 (Unauthorized):** If the user is not authenticated.

* **Example (curl):**

```
# Assuming you have a JWT access token stored in a variable named 'ACCESS_TOKEN'
curl -H "Authorization: Bearer $ACCESS_TOKEN" http://localhost:5000/User
```

#### UpdateUser (PUT /User)

* **Info:** Updates the profile information for the currently authenticated user.
* **Requirements:**  
    * User must be authenticated.
* **Request Body:**
   ```json
  {
    "FirstName": "string", (optional)
    "LastName": "string", (optional)
  }
  ```
* **Response:**
    * **200 (OK):** Returns the updated User object for the authenticated user.
    * **401 (Unauthorized):** If the user is not authenticated.
    * **404 (Not Found):** If the user is not found.

* **Example (curl):**

```
curl -X PUT -H "Authorization: Bearer $ACCESS_TOKEN" -H "Content-Type: application/json" -d '{ "FirstName": "John", "LastName": "Doe" }' http://localhost:5000/User
```

#### DeleteUser (DELETE /User)

* **Info:** Deletes the currently authenticated user and all associated TodoNotes.
* **Requirements:**  
    * User must be authenticated.
* **Request Body:**
   ```json
    No request body required.
   ```
* **Response:**
    * **204 (No Content):** If the user is deleted successfully.
    * **401 (Unauthorized):** If the user is not authenticated.

* **Example (curl):**

```
curl -X DELETE -H "Authorization: Bearer $ACCESS_TOKEN" http://localhost:5000/User
```

### AuthController

This controller handles user registration, login, and logout functionalities.

#### Register (POST /Auth/register)

* **Info:** Registers a new user.
* **Requirements:**  None (public endpoint).
* **Request Body:**
   ```json
  {
    "FirstName": "string" (required),
    "LastName": "string" (required),
    "Email": "string" (required),
    "Password": "string" (required - min length 8 characters)
  }
  ```
* **Response:**
    * **200 (OK):**  If the user is registered successfully.
    * **400 (Bad Request):**
        * If the request body is invalid or missing required fields.
        * If the user already exists with the provided email.
        * If the password is less than 8 characters.
        * Includes a message property detailing the reason for the bad request.

* **Example (curl):**

```
curl -X POST -H "Content-Type: application/json" -d '{ "FirstName": "John", "LastName": "Doe", "Email": "johndoe@example.com", "Password": "SecurePassword123" }' http://localhost:5000/Auth/register
```

#### Login (POST /Auth/login)

* **Info:** Logs in a registered user and generates a JWT token for authorization.
* **Requirements:**  None (public endpoint).
* **Request Body:**
   ```json
  {
    "Email": "string" (required),
    "Password": "string" (required)
  }
  ```
* **Response:**
    * **200 (OK):** Returns an object containing a "token" property with the generated JWT token upon successful login.
    * **400 (Bad Request):** If the request body is invalid or missing required fields.
    * **401 (Unauthorized):** If the user is not found or the password is incorrect.

* **Example (curl):**

```
curl -X POST -H "Content-Type: application/json" -d '{ "Email": "johndoe@example.com", "Password": "SecurePassword123" }' http://localhost:5000/Auth/login
```

**Note:** The provided Login response example only shows the successful login case where the token is returned. 

#### Logout (POST /Auth/logout)

* **Info:** Logs out the currently authenticated user.
* **Requirements:**  Requires a valid JWT token in the Authorization header (bearer token).
* **Request Body:**
   ```json
   ```
* **Response:**
    * **200 (OK):** If the user is successfully logged out.

* **Example (curl):**

```
# Assuming you have a JWT access token stored in a variable named 'ACCESS_TOKEN'
curl -X POST -H "Authorization: Bearer $ACCESS_TOKEN" http://localhost:5000/Auth/logout
```