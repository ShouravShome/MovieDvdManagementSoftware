using System;
using System.Reflection;
using System.Text.RegularExpressions;
using static System.Console;

interface iHashtable
{
    int Count //get the number of elements in the hashtable
    {
        get;
    }

    /* pre:  true
	* post: return the bucket where the key is stored
	*		 if the given key in the hashtable;
	*		 otherwise, return -1.
	*/
    int SearchUsingTitleInputToDisplatInformation(string key);


    /* pre:  true
	* post: the given key has been inserted into the hashtable, if it has not
	*/
    void Insert(string key, Movie value); //insert the key to the hashtable

    /* pre:  nil
	* post: the given key has been removed from the hashtable if the given key is in the hashtable
	*/
    int DeleteWholeMovieInformationUsingTitleInputByUser(int key);

    /* pre:  nil
	 * post: print all the elements in the hashtable
	*/
    void PrintAllMovies();
}
class Movie 
{
    private string title;
    private string genre;
    private string classification;
    private string duration;
    private int currentNumberOfDVDs;// available number of DVDs that can be borrowed
    private static int maximumNumberOfDVDs = 10; // maximum numbers of DVDs that could be added to the system
    private int numberOfDVDsCurrentlyBorrowed; // the number of DVDs that are borrowed by members right now
    private string[] nameOfMemberCurrentlyBorrowingThisMovie = new string[maximumNumberOfDVDs]; // array to hold the names of the members who are borrowing a certain movie
    private int totalNumberOfBorrowedFrequency=0; // increament whenever a movie is borrowed// use to figure out which movies are top most borrowed in this system
   

    public string Title {  get { return title; } set {  title = value; } }
    public string Genre { get {  return genre; } set {  genre = value; } }
    public string Classification { get {  return classification; } set {  classification = value; } }
    public string Duration { get { return duration; } set { duration = value; } }

    public Movie()//default constructor used for hashtable Movie objects
    {

    }
    public Movie(Movie top3movies)//this constructor is used for the top3Movies array. reason of using this constructor is to make sure that any modification of objects created with default constructor will not effect this constructor objects
    {
        Title = top3movies.Title;
        TotalNumberOfBorrowedFrequency = top3movies.TotalNumberOfBorrowedFrequency;
    }
    
    public int CurrentNumberOfDVDs 
    { 
        get { return currentNumberOfDVDs; } 
        set 
        { 
            if (value > maximumNumberOfDVDs)
            {
                throw new ArgumentException("More than 10"); //if staff wants to add more than the maximum movie number it will throw an exception. // current number of dvds cannot be higher than maximum number
            }
            else
            {
                currentNumberOfDVDs = value;
            }
            
        } 
    }
    public int MaximumNumberOfDVDs { get { return maximumNumberOfDVDs; } set { maximumNumberOfDVDs = value; } }
    public int NumberOfDVDsCurrentlyBorrowed { get { return numberOfDVDsCurrentlyBorrowed; } set { numberOfDVDsCurrentlyBorrowed = value; } }

    public int TotalNumberOfBorrowedFrequency { get { return totalNumberOfBorrowedFrequency; } set { totalNumberOfBorrowedFrequency = value; } }

    public string[] NameOfMemberCurrentlyBorrowingThisMovie { get { return nameOfMemberCurrentlyBorrowingThisMovie; } set { nameOfMemberCurrentlyBorrowingThisMovie= value; } }

    public void AddMemberFullnameToArrayMemberCurrentlyBorrowingThisMovie(string memberFullname)  // adding the names of the members who are borrowing a certain movie
    {
        int i = 0;
        while(i<numberOfDVDsCurrentlyBorrowed && nameOfMemberCurrentlyBorrowingThisMovie[i] != null)
        {
            i++;
        }
        nameOfMemberCurrentlyBorrowingThisMovie[i]=memberFullname;
        numberOfDVDsCurrentlyBorrowed++;
        currentNumberOfDVDs--;
        totalNumberOfBorrowedFrequency++;

    }

    public void RemoveMemberFullnameFromArrayMemberCurrentlyBorrowingThisMovie(string memberFullName) // removing the names of the members who are borrowing a certain movie
    {
        int i = 0;
        while (i < numberOfDVDsCurrentlyBorrowed && nameOfMemberCurrentlyBorrowingThisMovie[i] != memberFullName)
        {
            i++;
        }
        while (i < numberOfDVDsCurrentlyBorrowed)
        {
            nameOfMemberCurrentlyBorrowingThisMovie[i] = nameOfMemberCurrentlyBorrowingThisMovie[i + 1];
            i++;
        }
        nameOfMemberCurrentlyBorrowingThisMovie[i] = null;
        numberOfDVDsCurrentlyBorrowed--;
        currentNumberOfDVDs++;
    }

    public void PrintMembersNameBorrowingMovie(string userInputTitle) // print the name of the members who are borrowing a certain movie
    {
        WriteLine("      Currently "+numberOfDVDsCurrentlyBorrowed+" member(s) are borrowing " + userInputTitle.ToUpper() + " movie.      ");
        WriteLine("");
        int j = 1;
        for (int i = 0; i < numberOfDVDsCurrentlyBorrowed; i++)
        {
            WriteLine("");
            WriteLine("Full name " + j + ")  " + nameOfMemberCurrentlyBorrowingThisMovie[i]);
            WriteLine("");
            j++;
        }
        WriteLine("---------------------------------------------------------------------------------------------");
    }
}
class MovieCollection: iHashtable
{
    private int count; //the number of key-and-value pairs currently stored in the hashtable
    private int buckets = 1000; //number of buckets
    private Movie[] table; //a table storing key-and-value pairs. I decalre it as Movie object type array. Because every movie object gets added to table array. 
    private string deleted = "Movie Deleted By Staff";// value for the movie objects that are being deleted. The strings will be replaced by this.

    private Movie[] top3MoviesArray = new Movie[3];//to store top 3 movies based on their borrowed frequency

    public int Capacity
    {
        get { return buckets; }
        set { buckets = Capacity; }
    }

    public Movie[] Table { get { return table; } set { table = value; } }

    // constructor
    public MovieCollection()// intialising the hash table with null and the 3 movie type variables borrowed frequency to min value for comparison
    {
        
        count = 0;
        table = new Movie[buckets];
        for (int i = 0; i < buckets; i++)
            table[i] = null;
        Movie tempMovie = new Movie();//initializing top3Movies array elements with minimum int value for the comparison
        for(int i = 0; i < 3; i++)
        {
            top3MoviesArray[i] = new Movie(tempMovie);
            top3MoviesArray[i].Title = "";
            top3MoviesArray[i].TotalNumberOfBorrowedFrequency = int.MinValue;
        }
    }



    public int Count { get { return count; } }

    
    /* pre:  the hashtable is not full
	 * post: return the bucket for inserting the key
	 */
    private int FindInsertionBucket(string stringKeyMovieTitle)
    {
        int key = ConvertStringTitleToIntKey(stringKeyMovieTitle);//  get the key by converting the title to its respective ascii values
        int bucketNumber = Hashing(key);
        int i = 0;
        int offset = 499 - (key % 499);// second hash function for double probing
        while((i<buckets) && (table[(bucketNumber+(i*offset))%buckets]!=null) && (table[(bucketNumber + (i * offset)) % buckets].Title != deleted)) 
        {
            i++;
        }
        return (bucketNumber + (i * offset)) % buckets;
    }

    /* pre:  true
	* post: return the bucket where the key is stored
	*		 if the given key in the hashtable;
	*		 otherwise, return -1.
	*/
    public void Insert(string key, Movie value)
    {
        // check the pre-condition
        if ((Count < table.Length))
        {
            int bucketKey = FindInsertionBucket(key);
            table[bucketKey] = value;
            count++;
            FindTop3Movies(bucketKey);
        }
        else
            Console.WriteLine("The hashtable is full");

    }

    public int SearchUsingTitleInputToDisplatInformation(string userInputTitle)//searches the hashtable and return the index or key 
    {
        int key = ConvertStringTitleToIntKey(userInputTitle);
        int bucketKey = Hashing(key);
        int i = 0;
        int offset = 499 - (key % 499);
        while (i < buckets && (table[(bucketKey + (i * offset)) % buckets] != null) && (table[(bucketKey + (i * offset)) % buckets].Title != userInputTitle))
        {
            i++;
        }
        if ((table[(bucketKey + (i * offset)) % buckets] == null))
            return -1;
        else if (table[(bucketKey + (i * offset)) % buckets].Title == userInputTitle)
            return (bucketKey + (i * offset)) % buckets;
        else
            return -1;
    }

    /* pre:  nil
	 * post: the given key has been removed from the hashtable if the given key is in the hashtable
	*/

    public int DeleteWholeMovieInformationUsingTitleInputByUser(int bucketKey) //this is called to delete a whole movie object from the hash table
    //pass the bucket index here by explicitly searching for the index number
    
    {
        if (table[bucketKey].NumberOfDVDsCurrentlyBorrowed > 0)//if the movie is borrowed by a member the movie object cannot be deleted completely. value greater than 0 means currently borrowed
        {
            return -1;
        }
        else
        {
            table[bucketKey].Title = deleted;
            table[bucketKey].Genre = deleted;
            table[bucketKey].Classification = deleted;
            table[bucketKey].Duration = deleted;
            table[bucketKey].CurrentNumberOfDVDs = 0;
            table[bucketKey].TotalNumberOfBorrowedFrequency = 0;
            count--;
            return 1;
        }
    }
    /* pre:  key>=0
	 * post: return the bucket (location) for the given key
	 */

    private int Hashing(int key)
    {
        return (key % buckets);
    }

    /* pre:  nil
	 * post: print all the elements in the hashtable
	*/

    public void PrintAllMovies()// printing all the movies list
    {
        Movie movieList = new Movie();
        WriteLine("-------------------------------------------------");
        WriteLine("|        All The Movies Currently Available      |");
        WriteLine("-------------------------------------------------");
        WriteLine("");
        WriteLine("Total Number of Movies Available Right Now: " + count);
        WriteLine("");
        int j = 1;
        for (int i = 0; i < buckets; i++)
        {
            if ((table[i] == null) || table[i].Title==deleted)
            { }
            else
            {
                movieList = table[i];
                //WriteLine(i);
                WriteLine(j + ")  Movie Name: " + movieList.Title + " | Genre:  " + movieList.Genre + " | Number of copies available: "+movieList.CurrentNumberOfDVDs);
                WriteLine("-------------------------------------------------");
                j++;
            }
        }
    }

    public int UpdateNumberOfDVDsOfExistingMovie(int indexNumberOfBucketContainingMovieObjectDetails, int newNumberOfDVDs) // this method is called when the movie already
    // exist in the hash table and staff just wants to update the number of DVDs. hash table index and the new numbers of DVDs is passed in this method
    {
        try
        {
            table[indexNumberOfBucketContainingMovieObjectDetails].CurrentNumberOfDVDs = table[indexNumberOfBucketContainingMovieObjectDetails].CurrentNumberOfDVDs + newNumberOfDVDs + table[indexNumberOfBucketContainingMovieObjectDetails].NumberOfDVDsCurrentlyBorrowed;
            table[indexNumberOfBucketContainingMovieObjectDetails].CurrentNumberOfDVDs = table[indexNumberOfBucketContainingMovieObjectDetails].CurrentNumberOfDVDs - table[indexNumberOfBucketContainingMovieObjectDetails].NumberOfDVDsCurrentlyBorrowed;
            return 1;
        }
        catch(ArgumentException e)
        {
            return -1;
        }
    }

    public int DeleteNumberOfDVDsOfExistingMovie(int indexNumberOfBucketContainingMovieObjectDetails, int toBeDeletedNumberOfDVDs)// this method is called when the movie already
    // exist in the hash table and staff just wants to delete some numbers of DVDs. hash table index and the numbers of DVDs they want to delete is passed in this method
    {
        if (table[indexNumberOfBucketContainingMovieObjectDetails].CurrentNumberOfDVDs > toBeDeletedNumberOfDVDs)
        {
            table[indexNumberOfBucketContainingMovieObjectDetails].CurrentNumberOfDVDs = table[indexNumberOfBucketContainingMovieObjectDetails].CurrentNumberOfDVDs - toBeDeletedNumberOfDVDs;
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public int ConvertStringTitleToIntKey(string title)//method to convert the string title to a integer value
    {
        int hashValue = 0;
        for (int i = 0; i < title.Length; i++)
        {
            int result = (int)title[i];
            hashValue += result;
        }
        return hashValue;
    }

    public void PrintTop3Movies() // printing top 3 movies with title and their borrowed frequency number
    {
        WriteLine("");
        WriteLine("---------------------------------------------------------");
        WriteLine("                       Top 3 movies                     -");
        WriteLine("---------------------------------------------------------");
        for(int i =0; i < 3; i++)
        {
            WriteLine("||| Movie Title: " + top3MoviesArray[i].Title + " ||| Movie Borrowed total " + top3MoviesArray[i].TotalNumberOfBorrowedFrequency + " times...");
            WriteLine("");
        }
    }
    public void FindTop3Movies(int indexOfBucketHoldingMovie)// the algorihtm for keeping trak of the top 3 movies. this is called everytime when a movie is inserted or borrowed
    //input is the  index number of the hashtable element. by using the index number it gets the details of the movie object mainly the title and totalNumberOfBorrowedFrequency. 
    //an array named top3MoviesArray is used here to store the top 3 movies that has highest TotalNumberOfBorrowedFrequency among all the movies in hashtable
    {
        int flag = 0;//flag will be 1 if movie is already in the array which denotes that the movie already has highest number borrowing frequency and is borrowed once again.
        //flag 0 means the movie that is passed by the index is not currently in the array. So, algorithm will check if its borrowed frequency is increased than any of the existing movies in the array.
        
        for (int i = 2; i >= 0; i--)//iterates over the array to check if the elements is already in the array. 
        {
            if (top3MoviesArray[i].Title == table[indexOfBucketHoldingMovie].Title) 
            {
                flag = 1;
                if (i == 2)// i =2 denotes that the passed reference is found in 1st iteration meaning it the 2nd index or third value of the array. So if the value is larger than its previous elemets values than it will swap values.
                {
                    if (table[indexOfBucketHoldingMovie].TotalNumberOfBorrowedFrequency > top3MoviesArray[i - 1].TotalNumberOfBorrowedFrequency)//comparison with the previous index which means index 1 
                    {
                        if (table[indexOfBucketHoldingMovie].TotalNumberOfBorrowedFrequency > top3MoviesArray[i - 2].TotalNumberOfBorrowedFrequency) // comparison with 0th index 
                        {
                            // means that table[indexOfBucketHoldingMovie] macthes with index 2 and has the highest value so it needs to placed in 0 th index to maintain descending order
                            top3MoviesArray[i] = top3MoviesArray[i-1];//swapping till the object with highest borrowed frequency gets the 0th index position// index 2 is swapped with index 1 
                            top3MoviesArray[i-1] = top3MoviesArray[i-2];//index 1 with index 0
                            top3MoviesArray[i-2] = new Movie(table[indexOfBucketHoldingMovie]); // replace index 0 with constructor Movie(Movie top3movies) to store table object in top3MoviesArray . 
                            break;
                        }
                        else
                        {
                            // means that table[indexOfBucketHoldingMovie] macthes with index 2 and has larger value then index 1 but smaller then index 0 so it needs to placed in 1 th index to maintain descending order
                            top3MoviesArray[i] = top3MoviesArray[i - 1];//swap index 2 with index 1
                            top3MoviesArray[i-1] = new Movie(table[indexOfBucketHoldingMovie]);
                            break;
                        }
                    }
                    else
                    {
                        top3MoviesArray[i]= new Movie(table[indexOfBucketHoldingMovie]);//using the Movie(Movie top3movies) constructor
                        break;
                    }
                }
                else if( i == 1)// i=1 denotes 2nd iteration and passed reference is already in 1st index or 2nd element of the array
                {
                    if (table[indexOfBucketHoldingMovie].TotalNumberOfBorrowedFrequency > top3MoviesArray[i-1].TotalNumberOfBorrowedFrequency)//comparison with the privious index
                    {
                        // means that table[indexOfBucketHoldingMovie] macthes with index 1 and has larger value then index 0 so it needs to placed in 0 th index to maintain descending order
                        top3MoviesArray[i] = top3MoviesArray[i - 1];//swap
                        top3MoviesArray[i - 1] = new Movie(table[indexOfBucketHoldingMovie]);
                        break;
                    }
                    else
                    {
                        top3MoviesArray[i] = new Movie(table[indexOfBucketHoldingMovie]);//using the Movie(Movie top3movies) constructor
                        break;
                    }
                }
                else //i=0 denotes 3 rd iteration or the reference is already in 0th index or 1st element of the array
                {
                    top3MoviesArray[i] = new Movie(table[indexOfBucketHoldingMovie]);//using the Movie(Movie top3movies) constructor
                    break;
                }
            }
        }
        if(flag == 0)//when the reference object does not exist in the array. 
        {
            if (table[indexOfBucketHoldingMovie].TotalNumberOfBorrowedFrequency > top3MoviesArray[2].TotalNumberOfBorrowedFrequency)//checking if the refrence object has highes value than 2nd index of the array or 3rd element.
            {
                if (table[indexOfBucketHoldingMovie].TotalNumberOfBorrowedFrequency > top3MoviesArray[1].TotalNumberOfBorrowedFrequency)//comparison with 1st index or 2nd element
                {
                    if (table[indexOfBucketHoldingMovie].TotalNumberOfBorrowedFrequency > top3MoviesArray[0].TotalNumberOfBorrowedFrequency)//comparison with 0th index or 1st element
                    {
                        top3MoviesArray[2] = top3MoviesArray[1];//swap
                        top3MoviesArray[1] = top3MoviesArray[0];
                        top3MoviesArray[0] = new Movie(table[indexOfBucketHoldingMovie]);
                    }
                    else
                    {
                        top3MoviesArray[2] = top3MoviesArray[1];
                        top3MoviesArray[1] = new Movie(table[indexOfBucketHoldingMovie]);
                    }
                }
                else
                {
                    top3MoviesArray[2] = new Movie(table[indexOfBucketHoldingMovie]);
                }
            }
        }
    }
}

class Member 
{
    private string firstName;
    private string lastName;
    private string fullName;
    private string phoneNumber;
    private string password;
    private static int maxNumberOfBorrowedMovies = 5;// maximum number of movies one member can borrow
    private int countBorrowedMovies;// keeps count of how many movies member is currently borrowing.// ultimately this value increments when ever a movie title is added in the arrayToStoreTitleOfBorrowedMovies and decrements if a movie title is removed
    private string[] arrayToStoreTitleOfBorrowedMovies=new string[maxNumberOfBorrowedMovies];// because maximum number of movies one member can borrow is 5..


    public string FirstName { get { return firstName; } set { firstName = value; } }
    public string LastName { get { return lastName; } set { lastName = value; } }
    public string PhoneNumber { get { return phoneNumber; } set { phoneNumber = value; } }
    public string Password { get { return password; } set { password = value; } }

    public string FullName { get { return fullName; } set { fullName = value; } }
    public int CountBorrowedMovies { get { return countBorrowedMovies; } set { countBorrowedMovies = value; } }

    public string[] ArrayToStoreTitleOfBorrowedMovies { get { return arrayToStoreTitleOfBorrowedMovies; } set { arrayToStoreTitleOfBorrowedMovies = value; } }

    public int AddBorrowedMoviesNameToArray(string inputTitle)// adding the movie titles to the array for keeping track of how many and which movis are borrowed
    {
        if(countBorrowedMovies==maxNumberOfBorrowedMovies)
        {
            return -1;
        }
        else
        {
            int i = 0;
            while (i<countBorrowedMovies && arrayToStoreTitleOfBorrowedMovies[i] != inputTitle)
            {
                i++;
            }
            if (arrayToStoreTitleOfBorrowedMovies[i] == null)
            {
                arrayToStoreTitleOfBorrowedMovies[i] = inputTitle;
                countBorrowedMovies++;
                return 1;
            }
            else
            {
                return -2;
            }
        }
    }

    public int SearchArrayOfBorrowedMovies(string movieTitle)// the search function is used to look if the movie title already exist in the array
        //which means memeber is already borrowing the movie and cannot borrow another copy of the movie
    {
        int i = 0;
        while(i<countBorrowedMovies-1 && arrayToStoreTitleOfBorrowedMovies[i] != movieTitle)
        {
            i++;
        }
        if (arrayToStoreTitleOfBorrowedMovies[i] == movieTitle)
        {
            return i;
        }
        else
        {
            return -1;
        }
    }
    public void PrintListBorrowedMovies()// print the movies that a member is currently borrowing
    {
        WriteLine("-------------------------------------------------");
        WriteLine("|        All The Currently Borrowed Movies      |");
        WriteLine("-------------------------------------------------");
        WriteLine("");
        WriteLine("Total Number of Movies Borrowed Right Now: " + countBorrowedMovies);
        WriteLine("");
        int j = 1;
        for (int i = 0; i < countBorrowedMovies; i++)
        {
            WriteLine("");
            WriteLine("Borrowed Movies "+j+" Title: "+ arrayToStoreTitleOfBorrowedMovies[i]);
            WriteLine("");
            j++;
        }
        WriteLine("---------------------------");
    }
    public void RemoveMovieTitleFromArray(int index) //while returning the borrowed movie this method is called to remove the movie tile from the array. input is the array index.
    {
        int i = index;
        while (i < countBorrowedMovies - 1)
        {
            arrayToStoreTitleOfBorrowedMovies[i] = arrayToStoreTitleOfBorrowedMovies[i + 1];
            i++;
        }
        arrayToStoreTitleOfBorrowedMovies[i] = null;
        countBorrowedMovies--;
    }
}

class MemberCollection 
{
    private Member[] arrayToHoldMemberObject;// array holding member object
    private int maxmumCapacity=1000;//maximum size of the array
    private int currentMembersCount = 0;//current number of members in the array
    public Member[] ArrayToHoldMemberObject { get { return arrayToHoldMemberObject; } set { arrayToHoldMemberObject = value; } }

    public int CurrentMembersCount { get { return currentMembersCount; } }

    public MemberCollection()//initializing the array as null
    {
        currentMembersCount = 0;
        arrayToHoldMemberObject = new Member[maxmumCapacity];
        for (int i = 0; i < maxmumCapacity; i++)
            arrayToHoldMemberObject[i] = null;
    }
    public void InsertMemberInArray(string fullNameOfMemberToBeAdded, Member objectHoldingMemberDetails)// to insert member in the array in ascending order insertion sort is perfromed
    {
        
        if (currentMembersCount > maxmumCapacity)// when the array reaches maximum number
        {
            WriteLine("table already full");
        }
        else
        {
            
            if (currentMembersCount == 0)// when there is no member the first member to be added is by default in a sorted position
            {
                arrayToHoldMemberObject[0] = objectHoldingMemberDetails;
                //return array;

            }
            else if (currentMembersCount == 1)// if there are 2 members, the member's full names are compared which goes in first index and which comes in second. (ascending)
            {
                if (arrayToHoldMemberObject[0].FullName.CompareTo(fullNameOfMemberToBeAdded) == 1)
                {
                    arrayToHoldMemberObject[1] = arrayToHoldMemberObject[0];
                    arrayToHoldMemberObject[0] = objectHoldingMemberDetails;
                    //return array;
                }
                else
                {
                    arrayToHoldMemberObject[1] = objectHoldingMemberDetails;
                    //return array;
                }
            }
            else// when more than 2 members, insertion sort is performed to maintain ascending order. 
            {
                int flag = 0;
                for (int j = currentMembersCount - 1; j >= 0 && flag == 0; j--)
                {
                    if (arrayToHoldMemberObject[j].FullName.CompareTo(fullNameOfMemberToBeAdded) == 1)
                    {
                        arrayToHoldMemberObject[j + 1] = arrayToHoldMemberObject[j];
                        arrayToHoldMemberObject[j] = objectHoldingMemberDetails;
                    }
                    else
                    {
                        arrayToHoldMemberObject[(j + 1)] = objectHoldingMemberDetails;
                        flag = 1;
                    }
                }
                //return array;
            }
            currentMembersCount++;
        }
    }

    public int SearchIfMemberAlreadyExist(string staffInputOfMemberFullName)// if the member already exist cannot enter the same member again/ input is the fullname of the member
    {   
        int i = 0;
        while (i < currentMembersCount && arrayToHoldMemberObject[i].FullName != staffInputOfMemberFullName)
        {
            i++;
        }
        if (arrayToHoldMemberObject[i] == null)
        {
            return -1;// by default array is initialised as null. so if the index has null value it means no object was found matching full name what user entered.
        }
        else if (arrayToHoldMemberObject[i].FullName == staffInputOfMemberFullName)
        {
            return i;
        }
        
        else
        {
            return -1;//Does not exist. 
        }
    }

    public void PrintMemberList()// print the list of all the existing members
    {
        Member memberList = new Member();
        WriteLine("-------------------------------------------------");
        WriteLine("|        All The Member Currently Registered    |");
        WriteLine("-------------------------------------------------");
        WriteLine("");
        WriteLine("Total Number of Members Available Right Now: " + currentMembersCount);
        WriteLine("");
        int j = 1;
        for (int i = 0; i < maxmumCapacity; i++)
        {
            if ((arrayToHoldMemberObject[i] == null))
            { }
            else
            {
                memberList = arrayToHoldMemberObject[i];
                //WriteLine(i);
                WriteLine(j + ")  First Name: " + memberList.FirstName + " | LastName: " + memberList.LastName + " | Full Name: " + memberList.FullName +" | Contact Number: " + memberList.PhoneNumber);
                WriteLine("-------------------------------------------------");
                j++;
            }
        }
    }


    public Member CheckPasswordMatch(int indexNumberOfElementFullnameMatchingWithInputFullname, string passwordInputByUser)// this checks the password if it matches. input is the index number of array holding member object and password that is enterd by the user runtime
    {
        if (arrayToHoldMemberObject[indexNumberOfElementFullnameMatchingWithInputFullname].Password != passwordInputByUser)
        {
            return null;
        }
        else
        {
            return arrayToHoldMemberObject[indexNumberOfElementFullnameMatchingWithInputFullname];
        }
    }

    public int DeleteMember(int index)// after searching explicitly the index number is passed here. checkes if member has any borrowed movies in their list. if not remove the member
    {
        Member member = arrayToHoldMemberObject[index];
        if(member.CountBorrowedMovies > 0)//checking for exisiting movies
        {
            return -1;
        }
        else
        {
            int i = index;
            while (i < currentMembersCount - 1)
            {
                arrayToHoldMemberObject[i] = arrayToHoldMemberObject[i + 1];
                i++;
            }
            arrayToHoldMemberObject[i] = null;
            currentMembersCount--;
            return 1;
        }
        
    }
}

internal class Program
{
    private static void Main(string[] args)
    {           
        ////////Load some hard coded movies and passing key_item(object) pair to MovieCollection class to insert the movies
        /////As object i directly pass the movie object so that i can access the values using Movie class properties like title, genre etc.
        string[,] hardcodedData = 
        {
            { "Tenet", "Action", "M15+", "150 minutes", "3" ,"Shourav Shome","1","1"},
            {"Inception", "Action", "MA15+", "160 minutes", "5", "Mitchell Johnson", "1", "1"},
            {"Dune", "Adventure", "MA15+", "166 minutes", "6", "Shoun Shome", "1", "1"},
            {"Interstellar", "Adventure", "MA15+", "170 minutes", "9", "Shane Warne", "1", "1"},
            {"Avatar", "Action", "MA15+", "162 minutes", "7", "Adam Zampa", "1", "1"}
        };
        MovieCollection movieCollection = new MovieCollection();
        string title = "";
        for (int i = 0; i < hardcodedData.GetLength(0); i++)
        {
            title = hardcodedData[i, 0].ToUpper();
            Movie movie = new Movie();
            movie.Title = title = hardcodedData[i, 0].ToUpper();
            movie.Genre = hardcodedData[i, 1];
            movie.Classification = hardcodedData[i, 2];
            movie.Duration = hardcodedData[i, 3];
            movie.CurrentNumberOfDVDs = int.Parse(hardcodedData[i, 4]);
            movie.NameOfMemberCurrentlyBorrowingThisMovie[0] = hardcodedData[i, 5].ToUpper();
            movie.NumberOfDVDsCurrentlyBorrowed = int.Parse(hardcodedData[i, 6]);
            movie.TotalNumberOfBorrowedFrequency = int.Parse(hardcodedData[i, 7]);
            //movieCollection.KeyTitle = title;
            if (movieCollection.SearchUsingTitleInputToDisplatInformation(title) == -1)
            {
                movieCollection.Insert(title, movie);
            }
            else
            {
                WriteLine("Movie already exist");
            }
            
        }
        //hardcoded member data
        string[,] hardcodedDataMember=
        {
            { "Shourav", "Shome","1234567890","shom","Tenet","1"},
            {  "Shoun", "Shome", "0123456789","sean","Dune","1" },
            { "Adam", "Zampa","0987654321","adam","Avatar","1"},
            { "Mitchell", "Johnson","9876543210", "mitc","Inception","1"},
            { "Shane", "Warne","5432101234", "shan","Interstellar","1"}
        };
        MemberCollection memberCollection = new MemberCollection();
        string fullName = "";
        for (int i = 0; i < hardcodedDataMember.GetLength(0); i++)
        {
            //fullName = hardcodedDataMember[i, 0];
            Member member = new Member();
            member.FirstName = hardcodedDataMember[i, 0].ToUpper();
            member.LastName = hardcodedDataMember[i, 1].ToUpper();
            member.FullName = fullName = hardcodedDataMember[i, 0].ToUpper()+" "+hardcodedDataMember[i,1].ToUpper();
            member.PhoneNumber = hardcodedDataMember[i, 2];
            member.Password = hardcodedDataMember[i, 3];
            member.ArrayToStoreTitleOfBorrowedMovies[0] = hardcodedDataMember[i, 4].ToUpper();
            member.CountBorrowedMovies = int.Parse(hardcodedDataMember[i, 5]);
            if (memberCollection.SearchIfMemberAlreadyExist(fullName) == -1)
            {
                memberCollection.InsertMemberInArray(fullName, member);
            }
            else
            {
                WriteLine("Member already exist");
            }

        }
        


        //mainMenu has all the menus of the main screen. displayMenuForStaff has all the menus for Staff. displayMenuForMember has all the menus for Member. 
        MainMenu();

        void MainMenu()
        {
            string userInput;
            bool invalidUserInput = true;//for checking if user is putting correct value in menu selection;
            WriteLine("======================================================================");
            WriteLine("COMMUNITY LIBRARY MOVIE DVD MANAGEMENT SYSTEM");
            WriteLine("======================================================================");
            WriteLine("");
            WriteLine("");
            WriteLine("Main Menu");
            WriteLine("----------------------------------------------------------------------");
            WriteLine("Select from the following:");
            WriteLine("");
            WriteLine("1. Staff");
            WriteLine("2. Member");
            WriteLine("0. End the program");
            Write("Enter your choice ==> ");
            userInput = ReadLine();
            //Make sure system asks the user to put the correct input each time user puts an invalid value.
            while (invalidUserInput == true)
            {

                if (userInput == "1")
                {
                    CheckStaffLogin();
                    invalidUserInput = false;
                }
                else if (userInput == "2")
                {
                    CheckMemberLogin();
                    invalidUserInput = false;
                }
                else if (userInput == "0")
                {
                    WriteLine("");
                    WriteLine("");
                    WriteLine("");
                    WriteLine("======================================================================");
                    WriteLine("=                Thank you for using our service                     =");
                    WriteLine("=                        Closing Program.....                        =");
                    WriteLine("======================================================================");
                    WriteLine("");
                    WriteLine("");
                    WriteLine("");
                    Environment.Exit(0);
                }
                else
                {
                    Write("Please select correct input ==> ");
                    userInput = ReadLine();
                }
            }
        }
        void DisplayMenuForStaff(){
            WriteLine("");
            WriteLine("");
            WriteLine("");
            WriteLine("-------------------------------------------------");
            WriteLine("-                 Staff Menu                    -");
            WriteLine("-------------------------------------------------");
            WriteLine("1. Add DVDs to System");
            WriteLine("2. Remove DVDs from System");
            WriteLine("3. Register a new Member to System");
            WriteLine("4. Remove a registered Member from System");
            WriteLine("5. Find a Member contact phone number, given the Member's name");
            WriteLine("6. Find Members who are currently renting a particular Movie");
            WriteLine("0. Return to main menu");
            Write("Enter your choice ==> ");
            string userInput = "";
            bool invalidUserInput=true;
            userInput = ReadLine();
            while (invalidUserInput==true){
                if (userInput == "1")
                {
                    AddNewMovieDVD();
                    invalidUserInput = false;
                }
                else if (userInput == "2")
                {
                    RemoveMovieDVDs();
                    invalidUserInput = false;
                }
                else if (userInput == "3")
                {
                    AddNewMember();
                    invalidUserInput = false;
                }
                else if (userInput == "4")
                {
                    RemoveMember();
                    invalidUserInput = false;
                }
                else if (userInput == "5")
                {
                    FindContactNumber();
                    invalidUserInput = false;
                }
                else if (userInput == "6")
                {
                    MembersBorrowingCertainMovie();
                    invalidUserInput = false;
                }
                else if (userInput == "0")
                {
                    invalidUserInput = false;
                    MainMenu();

                }
                else 
                {
                    Write("Please select correct input ==> ");
                    userInput = ReadLine();
                }
            }

        }
        void DisplayMenuForMember(Member member)
            {
            WriteLine("");
            WriteLine("---------------------------------------------------------------------------------------------");
            WriteLine("-   Welcome " + member.FullName + " to Movie DVD Management System   -");
            WriteLine("---------------------------------------------------------------------------------------------");
            WriteLine("");
            WriteLine("1. Browse all the Movies");
            WriteLine("2. Display all the information about a Movie, given the title of the Movie");
            WriteLine("3. Borrow a Movie DVD");
            WriteLine("4. Return a Movie DVD");
            WriteLine("5. List current borrowing Movies");
            WriteLine("6. Display the top 3 Movies rented by the Members");
            WriteLine("0. Return to main menu");
            Write("Enter your choice ==> ");
            string userInput = "";
            bool invalidUserInput = true;
            userInput = ReadLine();
            while (invalidUserInput == true)
            {
                if (userInput == "1")
                {
                    BrowseAllMovies();
                    DisplayMenuForMember(member);
                    invalidUserInput = false;
                }
                else if (userInput == "2")
                {
                    DisplatInformationOfMovieUsingTitle(member);
                    invalidUserInput = false;
                }
                else if (userInput == "3")
                {
                    BorrowMovie(member);
                    invalidUserInput = false;
                }
                else if (userInput == "4")
                {
                    ReturnMovie(member);
                    invalidUserInput = false;
                }
                else if (userInput == "5")
                {
                    ListCurrentlyBorrowedMovies(member);
                    invalidUserInput = false;
                }
                else if (userInput == "6")
                {
                    ListOfTop3BorrowedMovies();
                    invalidUserInput = false;
                    DisplayMenuForMember(member);
                }
                else if (userInput == "0")
                {
                    invalidUserInput = false;
                    MainMenu();
                }
                else
                {
                    Write("Please select correct input ==> ");
                    userInput = ReadLine();
                }
            }

        }
        
        //Validation function to check user logins
        void CheckMemberLogin()
        {
            string userInputFirstName = "";
            string userInputLastName = "";
            WriteLine("---------------------------------------------------------------");
            WriteLine("-              Welcome to Member Login Screen                 -");
            WriteLine("---------------------------------------------------------------");
            WriteLine("Please provide the details that is asked to login..");
            WriteLine("");
            

            static bool NameValidation(string input)//validation to ensure no spaces or special characters exist in user input for first name and last name.
            {
                // Regular expression pattern to match only letters
                string pattern = @"^[a-zA-Z]+$";
                return Regex.IsMatch(input, pattern);
            }

            addFirstName();

            void addFirstName()
            {
                bool invalidUserInput = true;
                WriteLine("");
                Write("First Name (without any space or special characters) or 0 to go back: ");
                userInputFirstName = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputFirstName == "0")
                    {
                        MainMenu();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputFirstName))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank please try again!");
                        Write("First Name (without any space or special characters) or 0 to go back: ");
                        userInputFirstName = ReadLine();
                    }
                    else if (NameValidation(userInputFirstName))
                    {
                        addLastName();
                        invalidUserInput = false;
                    }
                    else
                    {
                        WriteLine("");
                        WriteLine("Something wrong with input. Check if there are any spaces or special characters!");
                        Write("First Name (without any space or special characters) or 0 to go back: ");
                        userInputFirstName = ReadLine();
                    }
                }
            }

            void addLastName()
            {
                bool invalidUserInput = true;
                WriteLine(" ");
                Write("Last Name (without any space or special characters) or 0 to go back: ");
                userInputLastName = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputLastName == "0")
                    {
                        MainMenu();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputLastName))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank. Try again.");
                        Write("First Name (without any space or special characters) or 0 to go back: ");
                        userInputLastName = ReadLine();
                    }
                    else if (NameValidation(userInputLastName))
                    {
                        string fullName = userInputFirstName.ToUpper() + " " + userInputLastName.ToUpper();
                        int indexNumberOfElementMatchingWithInput = memberCollection.SearchIfMemberAlreadyExist(fullName);//checks if the name is registered as a member
                        if (indexNumberOfElementMatchingWithInput == -1)
                        {
                            WriteLine("");
                            WriteLine("-----------------------------------------------------------------------------------");
                            WriteLine("-          Sorry Member Not Found with this First name and Last Name              -");
                            WriteLine("-----------------------------------------------------------------------------------");
                            WriteLine("");
                            WriteLine("Please Try Again");
                            WriteLine("");
                            CheckMemberLogin();
                            invalidUserInput = false;
                        }
                        else
                        {
                            addPassword(indexNumberOfElementMatchingWithInput);
                            invalidUserInput = false;
                        }
                    }
                    else
                    {
                        WriteLine("");
                        WriteLine("Something wrong with input. Check if there are any spaces or special characters!");
                        Write("Last Name (without any space or special characters) or 0 to go back: ");
                        userInputLastName = ReadLine();
                    }
                }
            }

            void addPassword(int indexNumberOfElementMatchingWithInput)
            {
                string userInputPassword = "";
                bool invalidUserInput = true;
                WriteLine("");
                Write("Password (please input only 4 character or numbers) or 0 to go back: ");
                userInputPassword = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputPassword == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputPassword))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank. Try again.");
                        Write("Password (please input only 4 character or numbers) or 0 to go back: ");
                        userInputPassword = ReadLine();
                    }
                    else if (userInputPassword.Length != 4)
                    {
                        WriteLine("");
                        WriteLine("Input cannot be larger than 4 character or smaller than 4 characters. Try again.");
                        Write("Password (please input only 4 character or numbers) or 0 to go back: ");
                        userInputPassword = ReadLine();
                    }
                    else
                    {
                        Member objectOfCurrentLoogedInMember = memberCollection.CheckPasswordMatch(indexNumberOfElementMatchingWithInput, userInputPassword);//checks if the password user has entered matches with the password of member's password
                        if(objectOfCurrentLoogedInMember != null)
                        {
                            DisplayMenuForMember(objectOfCurrentLoogedInMember);
                            invalidUserInput = false;
                        }
                        else
                        {
                            WriteLine("");
                            WriteLine("---------------------------------------------------------------------------------------------");
                            WriteLine("-           Your Password Do not Match. Please Try again putting your password              -");
                            WriteLine("---------------------------------------------------------------------------------------------");
                            WriteLine("");
                            addPassword(indexNumberOfElementMatchingWithInput);
                            invalidUserInput = false;
                        }
                    }
                }
            }
        }
        void CheckStaffLogin()
        {
            string staffUsername = "staff";
            string staffPassword = "today123";
            string userInputStaffName = "";
            string userInputStaffPassword = "";
            WriteLine("---------------------------------------------------------------");
            WriteLine("-              Welcome to Staff Login Screen                 -");
            WriteLine("---------------------------------------------------------------");
            WriteLine("Please provide the details that is asked to login..");
            WriteLine("");
            inputName();
            
            void inputName()
            {
                bool invalidUserInput = true;
                WriteLine("");
                Write("Staff UserName or 0 to go back: ");
                userInputStaffName = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputStaffName == "0")
                    {
                        MainMenu();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputStaffName))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank please try again!");
                        Write("Staff Name or 0 to go back: ");
                        userInputStaffName = ReadLine();
                    }
                    else
                    {
                        inputPassword();
                        invalidUserInput = false;
                    }
                }
            }
            void inputPassword()
            {
                bool invalidUserInput = true;
                WriteLine("");
                Write("Staff Password or 0 to go back: ");
                userInputStaffPassword = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputStaffPassword == "0")
                    {
                        MainMenu();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputStaffPassword))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank please try again!");
                        Write("Staff Name or 0 to go back: ");
                        userInputStaffPassword = ReadLine();
                    }
                    else
                    {
                        if(userInputStaffName == staffUsername && userInputStaffPassword == staffPassword)
                        {
                            WriteLine("Logged in Successfully"); 
                            DisplayMenuForStaff();
                        }
                        else
                        {
                            WriteLine("");
                            WriteLine("Sorry invalid username or password. Please try again!");
                            inputName();
                            invalidUserInput = false;
                        }
                        
                    }
                }
            }

        }

        //Start of all the functionalities


        //Member's function to load the list of all available movies
        void BrowseAllMovies()
        {
            movieCollection.PrintAllMovies();
        }

        void DisplatInformationOfMovieUsingTitle (Member member)
        {
            string userInput = "";
            bool invalidUserInput = true;
            WriteLine("");
            WriteLine("--------------------------------------------------");
            WriteLine("-Welcome to Movie Search using Title of the Movie-");
            WriteLine("--------------------------------------------------");
            Write("Please enter the name of the movie title or Press 0 to go back(movie title is case-insensitive): ");
            userInput = ReadLine();
            while(invalidUserInput == true)
            {
                if (userInput == "0")
                {
                    DisplayMenuForMember(member);
                    invalidUserInput=false;
                }
                else if (String.IsNullOrWhiteSpace(userInput))
                {
                    WriteLine("");
                    Write("Cannot search with empty title name! Try again with a valid input or 0 to go back: ");
                    userInput= ReadLine();
                }
                else
                {
                    int searchResult = movieCollection.SearchUsingTitleInputToDisplatInformation(userInput.ToUpper());// search is called to see if the movie exist in the hash table by the title.
                    //if exist use the index number to get all the details of that movie
                    if (searchResult == -1 )
                    {
                        WriteLine("");
                        WriteLine("--------------------------------------------------");
                        Write("Sorry no result found. Please try again with a different movie name: ");
                        userInput = ReadLine();
                    }
                    else
                    {
                        WriteLine("");
                        WriteLine("--------------------------------------------------");
                        WriteLine("-          Here is your search result            -");
                        WriteLine("--------------------------------------------------");
                        WriteLine("Title: "+ movieCollection.Table[searchResult].Title+"| Genre: "+ movieCollection.Table[searchResult].Genre+"| Classification: "+ movieCollection.Table[searchResult].Classification+"| Duration: "+ movieCollection.Table[searchResult].Duration+"| Number of DVDs: "+ movieCollection.Table[searchResult].CurrentNumberOfDVDs);
                        WriteLine("--------------------------------------------------");
                        WriteLine("");
                        WriteLine("Do you want to search for another movie?");
                        Write("Please enter the name of the movie title or Press 0 to go back(movie title is case-insensitive): ");
                        userInput = ReadLine();
                    }
                }
            }
        }

        void BorrowMovie(Member member)
        {
            string userInput = "";
            bool invalidUserInput = true;
            WriteLine("");
            WriteLine("----------------------------------------------------------------------------");
            WriteLine("-        Which movie do you want to borrow " + member.FirstName + " ?      -");
            WriteLine("----------------------------------------------------------------------------");
            WriteLine("");
            WriteLine("Please follow the instructions to borrow a movie. Thank you!");
            Write("Provide the movie title (case insensitive) or 0 to go back: ");
            userInput = ReadLine();
            while (invalidUserInput == true) 
            {
                if(userInput == "0")
                {
                    DisplayMenuForMember(member);
                    invalidUserInput = false;
                }
                else if (String.IsNullOrWhiteSpace(userInput))
                {
                    WriteLine("");
                    Write("Cannot search with empty title name! Try again with a valid input or 0 to go back: ");
                    userInput = ReadLine();
                }
                else
                {
                    int indexOfBucketContainingMovieTitleThatUserInputed = movieCollection.SearchUsingTitleInputToDisplatInformation(userInput.ToUpper());// search if the movie is present in hashtable

                    //if exist pass the index number.
                    // use the index number to get details of the movie from hash table
                    //store them in a movie type variable
                    if (indexOfBucketContainingMovieTitleThatUserInputed == -1)
                    {
                        WriteLine("");
                        WriteLine("----------------------------------------------------------------------------");
                        WriteLine("-               Sorry! The movie does not exist in our system              -");
                        WriteLine("----------------------------------------------------------------------------");
                        WriteLine("");
                        WriteLine("Please try again with other Movie title!");
                        WriteLine("");
                        Write("Provide the movie title (case insensitive) or 0 to go back: ");
                        userInput = ReadLine();
                    }
                    else
                    {
                        Movie objectOfMovieUserWantToBorrow = movieCollection.Table[indexOfBucketContainingMovieTitleThatUserInputed];// checks if the current number of dvds is smaller than or equal to 0
                        if (objectOfMovieUserWantToBorrow.CurrentNumberOfDVDs <= 0)
                        {
                            WriteLine("");
                            WriteLine("----------------------------------------------------------------------------");
                            WriteLine("-         Sorry! there are not enough copies available to borrow!!!        -");
                            WriteLine("-                         Please Check again later.                        -");
                            WriteLine("----------------------------------------------------------------------------");
                            WriteLine("");
                            WriteLine("Do you want to borrow another movie??");
                            WriteLine("");
                            Write("Provide the movie title (case insensitive) or 0 to go back: ");
                            userInput = ReadLine();
                        }
                        else
                        {
                            if (member.AddBorrowedMoviesNameToArray(userInput.ToUpper()) == 1)// the movie title is passed to the array that stores the borrowed movie names of the member
                                //if the method returns -1 it means the array is already full and cannot borrow more movie
                                //anything else than 1 or -1 means the title already exist in the member's borrowing array
                            {
                                WriteLine("");
                                WriteLine("----------------------------------------------------------------------------");
                                WriteLine("-         Congratulations! You have successfully borrowed the movie        -");
                                WriteLine("----------------------------------------------------------------------------");
                                WriteLine("");
                                WriteLine("Do you want to borrow another movie??");
                                WriteLine("");
                                objectOfMovieUserWantToBorrow.AddMemberFullnameToArrayMemberCurrentlyBorrowingThisMovie(member.FullName);
                                movieCollection.FindTop3Movies(indexOfBucketContainingMovieTitleThatUserInputed);
                                invalidUserInput = false;
                                BorrowMovie(member);

                            }
                            else if (member.AddBorrowedMoviesNameToArray(userInput.ToUpper()) == -1)
                            {
                                WriteLine("");
                                WriteLine("----------------------------------------------------------------------------");
                                WriteLine("-                Sorry! Cannot borrow more than 5 movies                   -");
                                WriteLine("----------------------------------------------------------------------------");
                                WriteLine("");
                                WriteLine("Please Return a movie to borrow another!");
                                WriteLine("");
                                invalidUserInput = false;
                                DisplayMenuForMember(member);
                            }
                            else
                            {
                                WriteLine("");
                                WriteLine("----------------------------------------------------------------------------");
                                WriteLine("-         Sorry! You have already borrowed one copy of this movie          -");
                                WriteLine("----------------------------------------------------------------------------");
                                WriteLine("");
                                WriteLine("Please Try Again with another movie title!");
                                WriteLine("");
                                Write("Provide the movie title (case insensitive) or 0 to go back: ");
                                userInput = ReadLine();
                            }
                        }
                    }
                }
            }
        }

        void ReturnMovie(Member member)
        {
            string userInput = "";
            bool invalidUserInput = true;
            WriteLine("");
            WriteLine("----------------------------------------------------------------------------");
            WriteLine("-        Which movie do you want to return " + member.FirstName + " ?      -");
            WriteLine("----------------------------------------------------------------------------");
            WriteLine("");
            WriteLine("Please follow the instructions to return a movie. Thank you!");
            Write("Provide the movie title (case insensitive) or 0 to go back: ");
            userInput = ReadLine();
            while (invalidUserInput == true)
            {
                if (userInput == "0")
                {
                    DisplayMenuForMember(member);
                    invalidUserInput = false;
                }
                else if (String.IsNullOrWhiteSpace(userInput))
                {
                    WriteLine("");
                    Write("Cannot search with empty title name! Try again with a valid input or 0 to go back: ");
                    userInput = ReadLine();
                }
                else
                {
                    int indexOfSearchedMovieInBorrowedMovieArray = member.SearchArrayOfBorrowedMovies(userInput.ToUpper());//checks if the movies exist in borrow list of member
                    //stores movie object if exists
                    if ( indexOfSearchedMovieInBorrowedMovieArray== -1)
                    {
                        WriteLine("");
                        WriteLine("----------------------------------------------------------------------------");
                        WriteLine("-           Sorry! The movie does not exist in your borrow list            -");
                        WriteLine("----------------------------------------------------------------------------");
                        WriteLine("");
                        WriteLine("Please try again with other Movie title!");
                        WriteLine("");
                        Write("Provide the movie title (case insensitive) or 0 to go back: ");
                        userInput = ReadLine();
                    }
                    else// pass the index number to remove method and removes the movie from members array to store movie names
                    {
                        int indexOfBucketContainingMovieTitleThatUserInputed = movieCollection.SearchUsingTitleInputToDisplatInformation(userInput.ToUpper());
                        Movie objectOfMovieUserWantToBorrow = movieCollection.Table[indexOfBucketContainingMovieTitleThatUserInputed];
                        member.RemoveMovieTitleFromArray(indexOfSearchedMovieInBorrowedMovieArray);
                        WriteLine("");
                        WriteLine("----------------------------------------------------------------------------");
                        WriteLine("-         Congratulations! You have successfully returned the movie       -");
                        WriteLine("----------------------------------------------------------------------------");
                        WriteLine("");
                        WriteLine("Do you want to return another movie??");
                        WriteLine("");
                        objectOfMovieUserWantToBorrow.RemoveMemberFullnameFromArrayMemberCurrentlyBorrowingThisMovie(member.FullName);
                        invalidUserInput = false;
                        ReturnMovie(member);
                    }
                }
            }
        }

        void ListCurrentlyBorrowedMovies(Member member)
        {
            member.PrintListBorrowedMovies();
            WriteLine("");
            DisplayMenuForMember(member);
        }
        
        void ListOfTop3BorrowedMovies()
        {
            movieCollection.PrintTop3Movies();
        }

        //Staff Functions

        void AddNewMovieDVD()
        {
            //these variables are used to temporarily track the details of movie object
            string userInputTitle = "";
            string userInputGenre = "";
            string userInputClassification = "";
            int userInputDurationInt = 0;
            int userInputNumberOfDVDCopiesInt = 0;
            WriteLine("");
            WriteLine("----------------------------------------------------------------------------------------------------------");
            WriteLine("-                  Add New Movies or Add DVD copies to already existing movies                           -");
            WriteLine("----------------------------------------------------------------------------------------------------------");
            WriteLine("Please enter all the details that is asked and do not leave the fields blank. Or enter 0 to go back");
            WriteLine("");
            movieCollection.PrintAllMovies();
            AddTitle();


            //this function takes title as input from user. And checks if the movie is already in MovieClassification hashtable. If it is there than skips taking input
            //for genre, classification and duration. instead directly takes the input of Number of DVD from user and adds it to previous value. 
            void AddTitle()
            {
                
                bool invalidUserInput = true;
                WriteLine("");
                Write("Movie Title (case insensitive) or 0 to go back: ");
                userInputTitle = ReadLine();
                while(invalidUserInput == true)
                {
                    if (userInputTitle == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputTitle))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank please try again!");
                        Write("Movie Title (case insensitive) or 0 to go back: ");
                        userInputTitle = ReadLine();
                    }
                    else
                    {
                        int indexOfBucketContainingMovieTitleThatUserInputed = movieCollection.SearchUsingTitleInputToDisplatInformation(userInputTitle.ToUpper());//search if the title already exist in hashtable
                        //if does not exist go to function where user can add genre of the movie. (addGenre)
                        //else go to addNumberOfDVDsAlreadyExistingMovies which will update the number of DVDs//the index number passed to the method
                        if (indexOfBucketContainingMovieTitleThatUserInputed == -1)
                        {
                            AddGenre();
                            invalidUserInput = false ;
                        }
                        else
                        {
                            WriteLine("");
                            WriteLine("--------------------------------------------------------------------------------------------------");
                            WriteLine("-      The Movie is already in the system. Just enter the number of DVDs you want to add         -");
                            WriteLine("--------------------------------------------------------------------------------------------------");
                            AddNumberOfDVDsAlreadyExistingMovies(indexOfBucketContainingMovieTitleThatUserInputed);
                        }
                    }
                }
            }

            void AddGenre()
            {
                
                bool invalidUserInput = true;
                string userChoice = "";
                WriteLine("");
                WriteLine("Select Movie Genre or 0 to go back: ");
                WriteLine("1. Drama");
                WriteLine("2. Adventure"); 
                WriteLine("3. Family");
                WriteLine("4. Action");
                WriteLine("5. Sci-fi");
                WriteLine("6. Comedy");
                WriteLine("7. Animated");
                WriteLine("8. Thriller");
                WriteLine("9. Other");
                Write("Select any: ");
                userChoice = ReadLine();
                while(invalidUserInput == true)
                {
                    if (userChoice == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userChoice))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank. Please Try Again: ");
                        WriteLine("Select Movie Genre or 0 to go back: ");
                        WriteLine("1. Drama");
                        WriteLine("2. Adventure");
                        WriteLine("3. Family");
                        WriteLine("4. Action");
                        WriteLine("5. Sci-fi");
                        WriteLine("6. Comedy");
                        WriteLine("7. Animated");
                        WriteLine("8.Thriller");
                        WriteLine("9. Other");
                        Write("Select any: ");
                        userChoice = ReadLine();
                    }
                    else if(userChoice == "1")
                    {
                        userInputGenre = "Drama";
                        AddClassification();
                        invalidUserInput = false;
                    }
                    else if (userChoice == "2")
                    {
                        userInputGenre = "Adventure";
                        AddClassification();
                        invalidUserInput = false;
                    }
                    else if (userChoice == "3")
                    {
                        userInputGenre = "Family";
                        AddClassification();
                        invalidUserInput = false;
                    }
                    else if (userChoice == "4")
                    {
                        userInputGenre = "Action";
                        AddClassification();
                        invalidUserInput = false;
                    }
                    else if (userChoice == "5")
                    {
                        userInputGenre = "Sci-fi";
                        AddClassification();
                        invalidUserInput = false;
                    }
                    else if (userChoice == "6")
                    {
                        userInputGenre = "Comedy";
                        AddClassification();
                        invalidUserInput = false;
                    }
                    else if (userChoice == "7")
                    {
                        userInputGenre = "Animated";
                        AddClassification();
                        invalidUserInput = false;
                    }
                    else if (userChoice == "8")
                    {
                        userInputGenre = "Thriller";
                        AddClassification();
                        invalidUserInput = false;
                    }
                    else if (userChoice == "9")
                    {
                        userInputGenre = "Other";
                        AddClassification();
                        invalidUserInput = false;
                    }
                    else
                    {
                        Write("Sorry invalid input! Please select any number between 1 to 9: ");
                        userChoice=ReadLine();
                    }
                }
            }

            void AddClassification()
            {
                bool invalidUserInput = true;
                string userChoice = "";
                WriteLine("");
                WriteLine("Select Movie Classification or 0 to go back: ");
                WriteLine("1. General (G)");
                WriteLine("2. Parental Guidance (PG)");
                WriteLine("3. Mature (M15+)");
                WriteLine("4. Mature Accompanied (MA15+)");
                Write("Select an option: ");
                userChoice = ReadLine();
                while(invalidUserInput == true)
                {
                    if (userChoice == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userChoice))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank. Try again: ");
                        Write("Select Movie Classification or 0 to go back: ");
                        userChoice = ReadLine();
                    }
                    else if(userChoice == "1")
                    {
                        userInputClassification = "General (G)";
                        AddDuration();
                        invalidUserInput = false;
                    }
                    else if (userChoice == "2")
                    {
                        userInputClassification = "Parental Guidance (PG)";
                        AddDuration();
                        invalidUserInput = false;
                    }
                    else if (userChoice == "3")
                    {
                        userInputClassification = "Mature (M15+)";
                        AddDuration();
                        invalidUserInput = false;
                    }
                    else if (userChoice == "4")
                    {
                        userInputClassification = "Mature Accompanied (MA15+)";
                        AddDuration();
                        invalidUserInput = false;
                    }
                    else
                    {
                        WriteLine("");
                        Write("Sorry invalid Input. Select options between 1 to 4: ");
                        userChoice=ReadLine();
                    }
                }

            }

            void AddDuration()
            {
                string userInputDurationString = "";
                bool invalidUserInput = true;
                WriteLine("");
                Write("Movie Duration in minutes(please input only numbers) or 0 to go back: ");
                userInputDurationString = ReadLine();
                while(invalidUserInput == true)
                {
                    if (userInputDurationString == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputDurationString))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank. Try again.");
                        Write("Movie Duration in minutes(please input only numbers) or 0 to go back: ");
                        userInputDurationString = ReadLine();
                    }
                    else
                    {
                        if(int.TryParse(userInputDurationString, out userInputDurationInt))//checking if user put numbers because duration is only shown in minutes
                        {
                            AddNumberOfDVDs();
                            invalidUserInput = false;
                        }
                        else
                        {
                            WriteLine("");
                            WriteLine("Input cannot be anything else than number. Try again.");
                            Write("Movie Duration in minutes(please input only numbers) or 0 to go back: ");
                            userInputDurationString = ReadLine();
                        }
                    }
                }
            }

            void AddNumberOfDVDs()
            {
                string userInputNumberOfDVDCopiesString = "";
                bool invalidUserInput = true;
                WriteLine("");
                Write("Number of DVDs(only in number and cannot be zero) or 0 to go back: ");
                userInputNumberOfDVDCopiesString = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputNumberOfDVDCopiesString == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputNumberOfDVDCopiesString))
                    {
                        WriteLine("");
                        WriteLine("Invalid input please try again");
                        Write("Number of DVDs(only in number and cannot be zero) or 0 to go back: ");
                        userInputNumberOfDVDCopiesString = ReadLine();
                    }
                    else
                    {
                        if (int.TryParse(userInputNumberOfDVDCopiesString, out userInputNumberOfDVDCopiesInt))
                        {
                            try
                            {
                                //movie object is called to store the movie object in hash table of movieCollection. 
                                Movie movie = new Movie();
                                movie.Title = userInputTitle.ToUpper();
                                movie.Genre = userInputGenre;
                                movie.Classification = userInputClassification;
                                movie.Duration = userInputDurationInt + " minutes";
                                movie.CurrentNumberOfDVDs = userInputNumberOfDVDCopiesInt;
                                movieCollection.Insert(userInputTitle.ToUpper(), movie);
                                WriteLine("");
                                WriteLine("---------------------------------------------------");
                                WriteLine("- The Movie DVD is successfully added. Thank you. - ");
                                WriteLine("---------------------------------------------------");
                                WriteLine("");
                                WriteLine("Do you want to add another movie?");
                                WriteLine("");
                                WriteLine("");
                                AddNewMovieDVD();
                                invalidUserInput = false;
                            }
                            catch(ArgumentException e)//staff cannot add more vidoes than the max number of dvds the system can have
                            {
                                WriteLine("");
                                WriteLine("---------------------------------------------------");
                                WriteLine("-  The Number Cannot be more than 10. Try again   -");
                                WriteLine("---------------------------------------------------");
                                WriteLine("");
                                Write("Number of DVDs(only in number and cannot be zero) or 0 to go back: ");
                                userInputNumberOfDVDCopiesString = ReadLine();
                            }
                        }
                        else
                        {
                            WriteLine("");
                            WriteLine("Invalid input please try again");
                            Write("Number of DVDs(only in number and cannot be zero) or 0 to go back: ");
                            userInputNumberOfDVDCopiesString = ReadLine();
                        }
                    }
                }
            }

            //It uses the index value of the bucket that holds the title that user wanted to add. and puts it in MovieCollection table array to directly add the number of dvds to
            //to the previous value.
            void AddNumberOfDVDsAlreadyExistingMovies(int indexOfBucketContainingMovieTitleThatUserInputed)
            {
                string userInputNumberOfDVDCopiesString = "";
                bool invalidUserInput = true;
                WriteLine("");
                Write("Number of DVDs(only in number and cannot be zero) or 0 to go back: ");
                userInputNumberOfDVDCopiesString = ReadLine();
                while (invalidUserInput == true)
                {
                    if(userInputNumberOfDVDCopiesString == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if(String.IsNullOrWhiteSpace(userInputNumberOfDVDCopiesString))
                    {
                        WriteLine("");
                        WriteLine("Invalid input please try again");
                        Write("Number of DVDs(only in number and cannot be zero) or 0 to go back: ");
                        userInputNumberOfDVDCopiesString = ReadLine();
                    }
                    else
                    {
                        if (int.TryParse(userInputNumberOfDVDCopiesString, out userInputNumberOfDVDCopiesInt))
                        {

                            //movieCollection.Table[indexOfBucketContainingMovieTitleThatUserInputed].NumberOfDVDs = movieCollection.Table[indexOfBucketContainingMovieTitleThatUserInputed].NumberOfDVDs + numberOfDVDCopies;
                            if(movieCollection.UpdateNumberOfDVDsOfExistingMovie(indexOfBucketContainingMovieTitleThatUserInputed,userInputNumberOfDVDCopiesInt) == 1)//the index and the number is passed to update the number of current DVDs
                            {
                                WriteLine("");
                                WriteLine("------------------------------------------------------------------");
                                WriteLine("-          The Number of DVDs are updated. Thank you.            -");
                                WriteLine("-             Do you want to add another movie?                  -");
                                WriteLine("------------------------------------------------------------------");
                                WriteLine("");
                                AddNewMovieDVD();
                                invalidUserInput = false;
                            }
                            else
                            {
                                WriteLine("");
                                WriteLine("---------------------------------------------------");
                                WriteLine("-  The Number Cannot be more than 10. Try again   -");
                                WriteLine("---------------------------------------------------");
                                WriteLine("");
                                Write("Number of DVDs(only in number and cannot be zero) or 0 to go back: ");
                                userInputNumberOfDVDCopiesString = ReadLine();
                            }
                        }
                        else
                        {
                            WriteLine("");
                            WriteLine("Invalid input please try again");
                            Write("Number of DVDs(only in number and cannot be zero) or 0 to go back: ");
                            userInputNumberOfDVDCopiesString = ReadLine();
                        }
                    }
                }
            }
        }

        void RemoveMovieDVDs()
        {
            string userInput = "";
            bool invalidUserInput = true;
            WriteLine("--------------------------------------------------");
            WriteLine("-         Do you want to delete a movie?         -");
            WriteLine("--------------------------------------------------");
            WriteLine("");
            movieCollection.PrintAllMovies();
            WriteLine("");
            Write("Please enter the Title(case in-sensitive) and do not leave the fields blank. Or enter 0 to go back: ");
            userInput = ReadLine();
            while (invalidUserInput == true) 
            {
                if (userInput == "0")
                {
                    DisplayMenuForStaff();
                    invalidUserInput = false ;
                }
                else if (String.IsNullOrWhiteSpace(userInput))
                {
                    WriteLine("");
                    WriteLine("Please Do not leave the title name empty");
                    Write("Please enter the Title(case in-sensitive) and do not leave the fields blank. Or enter 0 to go back: ");
                    userInput=ReadLine();
                }
                else
                {
                    int indexOfTheBucketToDeleteMovie = movieCollection.SearchUsingTitleInputToDisplatInformation(userInput.ToUpper());//checks if the movie exist in hash table
                    // if exist pass the index number to userChoiceToDeleteWholeCollectionOrAnAmountOfDVDs
                    if (indexOfTheBucketToDeleteMovie == -1)
                    {
                        WriteLine("");
                        WriteLine("Sorry no movie matches the Title you entered");
                        Write("Please enter the Title(case in-sensitive) and do not leave the fields blank. Or enter 0 to go back: ");
                        userInput=ReadLine();
                    }
                    else
                    {
                        UserChoiceToDeleteWholeCollectionOrAnAmountOfDVDs(indexOfTheBucketToDeleteMovie);
                        invalidUserInput=false ;
                    }
                }
            }
            void UserChoiceToDeleteWholeCollectionOrAnAmountOfDVDs(int indexOfTheBucketToDeleteMovie)
            {
                //this method mainly decides if staff want to remove the complete movie or some number of DVD copies
                string userChoice = "";
                bool invalidUserInput = true ;
                WriteLine("");
                WriteLine("Do you want to Delete all the copies of the movie? or some DVD copies of the movie?");
                Write("To delete all the DVD copies and information of the movie enter 'Yes' or to delete an amount of DVD copies enter the number or 0 to go back: ");
                userChoice = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userChoice == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false ;
                    }
                    else if (String.IsNullOrWhiteSpace(userChoice)){
                        WriteLine("Please do not leave the input blank");
                        Write("To delete all the DVD copies and information of the movie enter 'Yes' or to delete an amount of DVD copies enter the number or 0 to go back: ");
                        userChoice = ReadLine();    
                    }
                    else if (userChoice == "Yes")
                    {
                        if(movieCollection.DeleteWholeMovieInformationUsingTitleInputByUser(indexOfTheBucketToDeleteMovie) == 1)
                        {
                            WriteLine("");
                            WriteLine("--------------------------------------------------");
                            WriteLine("-        Successfully Deleted the Movie!         -");
                            WriteLine("--------------------------------------------------");
                            WriteLine("");
                            RemoveMovieDVDs();
                            invalidUserInput = false;
                        }
                        else
                        {
                            WriteLine("");
                            WriteLine("--------------------------------------------------");
                            WriteLine("-        Sorry Cannot delete the movie           -");
                            WriteLine("-    Beacause someone borrowing the movie        -");
                            WriteLine("--------------------------------------------------");
                            WriteLine("");
                            RemoveMovieDVDs();
                            invalidUserInput = false;
                        }
                        
                    }
                    else
                    {
                        int numberOfDVDsToBeDeleted;
                        if(int.TryParse(userChoice, out numberOfDVDsToBeDeleted))
                        {
                            //if the number they want to remove is equal or more than current number of DVDs then staff cannot delete that number. 
                            int deleteSuccesful = movieCollection.DeleteNumberOfDVDsOfExistingMovie(indexOfTheBucketToDeleteMovie, numberOfDVDsToBeDeleted);
                            if (deleteSuccesful == 1) 
                            {
                                WriteLine("");
                                WriteLine("--------------------------------------------------");
                                WriteLine("-    Successfully Deleted the number of DVDs!    -");
                                WriteLine("--------------------------------------------------");
                                WriteLine("");
                                RemoveMovieDVDs();
                                invalidUserInput = false;
                            }
                            else
                            {
                                WriteLine("");
                                WriteLine("--------------------------------------------------------------------------------------");
                                WriteLine("-    The number of DVDs you want to delete is more or equal to the existing number!  -");
                                WriteLine("-              Please check the movie list and see the correct number!               -");
                                WriteLine("--------------------------------------------------------------------------------------");
                                WriteLine("");
                                movieCollection.PrintAllMovies();
                                WriteLine("");
                                Write("To delete all the DVD copies and information of the movie enter 'Yes' or to delete an amount of DVD copies enter the number or 0 to go back: ");
                                userChoice = ReadLine();
                            }
                        }
                        else
                        {
                            WriteLine("");
                            WriteLine("Please input a valid number.");
                            Write("To delete all the DVD copies and information of the movie enter 'Yes' or to delete an amount of DVD copies enter the number or 0 to go back: ");
                            userChoice = ReadLine();
                        }
                    }
                }
            }
        }

        void AddNewMember()//this method adds new member to the system. 
        {
            string userInputFirstName = "";
            string userInputLastName = "";
            string fullName = "";
            string userInputPhoneNumber = "";
            string userInputPassword = "";
            memberCollection.PrintMemberList();
            WriteLine("");
            WriteLine("--------------------------------------------------");
            WriteLine("-                Add New Member                  -");
            WriteLine("--------------------------------------------------");
            WriteLine("Please enter all the details that is asked and do not leave the fields blank. Or enter 0 to go back");
            WriteLine("");
            AddFirstName();

            static bool NameValidation(string input)//validation to ensure no spaces or special characters exist in user input for first name and last name.
            {
                // Regular expression pattern to match only letters
                string pattern = @"^[a-zA-Z]+$";
                return Regex.IsMatch(input, pattern);
            }

            static bool PhoneNumberValidation(string input)
            {
                // Regular expression pattern to match only numbers
                string pattern = @"^\d+$";
                return Regex.IsMatch(input, pattern);
            }

             
            void AddFirstName()
            {
                bool invalidUserInput = true;
                WriteLine("");
                Write("First Name (without any space or special characters) or 0 to go back: ");
                userInputFirstName = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputFirstName == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputFirstName))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank please try again!");
                        Write("First Name (without any space or special characters) or 0 to go back: ");
                        userInputFirstName = ReadLine();
                    }
                    else if (NameValidation(userInputFirstName))
                    {
                        AddLastName();
                        invalidUserInput = false;
                    }
                    else
                    {
                        WriteLine("");
                        WriteLine("Something wrong with input. Check if there are any spaces or special characters!");
                        Write("First Name (without any space or special characters) or 0 to go back: ");
                        userInputFirstName = ReadLine();
                    }
                }
            }

            void AddLastName()
            {
                bool invalidUserInput = true;
                WriteLine(" ");
                Write("Last Name (without any space or special characters) or 0 to go back: ");
                userInputLastName = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputLastName == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputLastName))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank. Try again.");
                        Write("Last Name (without any space or special characters) or 0 to go back: ");
                        userInputLastName = ReadLine();
                    }
                    else if (NameValidation(userInputLastName))//if both first and last name is valid input it combines first name and last name and gets full name
                        //then the full name is compared to check if the member already exist in the system. 
                    {
                        fullName = userInputFirstName.ToUpper() + " "+userInputLastName.ToUpper();
                        int indexNumberOfElementMatchingWithInput = memberCollection.SearchIfMemberAlreadyExist(fullName);
                        if (indexNumberOfElementMatchingWithInput == -1)
                        {
                            AddPhoneNumber();
                            invalidUserInput = false;
                        }
                        else
                        {
                            WriteLine("");
                            WriteLine("-----------------------------------------------------------------------------------");
                            WriteLine("-       Sorry Member already exist. Please try again adding another member.       -");
                            WriteLine("-----------------------------------------------------------------------------------");
                            WriteLine("");
                            AddNewMember();
                            invalidUserInput = false;
                        }
                    }
                    else
                    {
                        WriteLine("");
                        WriteLine("Something wrong with input. Check if there are any spaces or special characters!");
                        Write("Last Name (without any space or special characters) or 0 to go back: ");
                        userInputLastName = ReadLine();
                    }
                }
            }

            void AddPhoneNumber()
            {
                
                bool invalidUserInput = true;
                WriteLine("");
                Write("Enter Phone Number (only 10 numbers without any spcial characters or spaces) or 0 to go back: ");
                userInputPhoneNumber = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputPhoneNumber == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputPhoneNumber))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank. Try again.");
                        Write("Enter Phone Number (only 10 numbers without any spcial characters or spaces) or 0 to go back: ");
                        userInputPhoneNumber = ReadLine();
                    }
                    else if(userInputPhoneNumber.Length !=10)
                    {
                        WriteLine("");
                        WriteLine("Input can only be 10 numbers");
                        Write("Enter Phone Number (only 10 numbers without any spcial characters or spaces) or 0 to go back: ");
                        userInputPhoneNumber = ReadLine();
                    }
                    else
                    {
                        if(PhoneNumberValidation(userInputPhoneNumber))
                        {
                            AddPassword();
                            invalidUserInput = false;
                        }
                        else
                        {
                            WriteLine("");
                            WriteLine("Something wrong with input. Check if there are any spaces or special characters!");
                            Write("Enter Phone Number (only 10 numbers without any spcial characters or spaces) or 0 to go back: ");
                            userInputPhoneNumber = ReadLine();
                        }
                    }
                }
            }

            void AddPassword()
            {
                
                bool invalidUserInput = true;
                WriteLine("");
                Write("Password (please input only 4 character or numbers) or 0 to go back: ");
                userInputPassword = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputPassword == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputPassword))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank. Try again.");
                        Write("Password (please input only 4 character or numbers) or 0 to go back: ");
                        userInputPassword = ReadLine();
                    }
                    else if(userInputPassword.Length != 4)
                    {
                        WriteLine("");
                        WriteLine("Input cannot be larger than 4 character or smaller than 4 characters. Try again.");
                        Write("Password (please input only 4 character or numbers) or 0 to go back: ");
                        userInputPassword = ReadLine();
                    }
                    else
                    {//after giving the password this member object added to the array of memberCollection class
                        Member member = new Member();
                        member.FirstName = userInputFirstName.ToUpper();
                        member.LastName = userInputLastName.ToUpper();
                        member.FullName = fullName.ToUpper();
                        member.PhoneNumber = userInputPhoneNumber;
                        member.Password = userInputPassword;
                        memberCollection.InsertMemberInArray(fullName, member);
                        WriteLine("");
                        WriteLine("---------------------------------------------------");
                        WriteLine("-   The Member is successfully added. Thank you.  - ");
                        WriteLine("---------------------------------------------------");
                        WriteLine("");
                        WriteLine("Do you want to add another member?");
                        WriteLine("");
                        AddNewMember();
                        invalidUserInput = false;
                    }
                }
            }
        }

        void RemoveMember()//this method takes first name and last name as input from staff and remove the member from the system
        {
            string userInputFirstName = "";
            string userInputLastName = "";
            string fullName = "";
            memberCollection.PrintMemberList();
            WriteLine("");
            WriteLine("--------------------------------------------------");
            WriteLine("-             Remove existing Member             -");
            WriteLine("--------------------------------------------------");
            WriteLine("Please enter all the details that is asked and do not leave the fields blank. Or enter 0 to go back");
            WriteLine("");
            AddFirstName();

            static bool NameValidation(string input)//validation to ensure no spaces or special characters exist in user input for first name and last name.
            {
                // Regular expression pattern to match only letters
                string pattern = @"^[a-zA-Z]+$";
                return Regex.IsMatch(input, pattern);
            }

            
            void AddFirstName()
            {
                bool invalidUserInput = true;
                WriteLine("");
                Write("First Name (without any space or special characters) or 0 to go back: ");
                userInputFirstName = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputFirstName == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputFirstName))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank please try again!");
                        Write("First Name (without any space or special characters) or 0 to go back: ");
                        userInputFirstName = ReadLine();
                    }
                    else if (NameValidation(userInputFirstName))
                    {
                        AddLastName();
                        invalidUserInput = false;
                    }
                    else
                    {
                        WriteLine("");
                        WriteLine("Something wrong with input. Check if there are any spaces or special characters!");
                        Write("First Name (without any space or special characters) or 0 to go back: ");
                        userInputFirstName = ReadLine();
                    }
                }
            }

            void AddLastName()//after getting the last name it combines first and last name. Then get the full name
                //checks if the member exist in system
            {
                bool invalidUserInput = true;
                WriteLine(" ");
                Write("Last Name (without any space or special characters) or 0 to go back: ");
                userInputLastName = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputLastName == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputLastName))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank. Try again.");
                        Write("Last Name (without any space or special characters) or 0 to go back: ");
                        userInputLastName = ReadLine();
                    }
                    else if (NameValidation(userInputLastName))
                    {
                        fullName = userInputFirstName.ToUpper() + " " + userInputLastName.ToUpper();
                        int indexNumberOfElementMatchingWithInput = memberCollection.SearchIfMemberAlreadyExist(fullName);
                        if (indexNumberOfElementMatchingWithInput == -1)
                        {
                            WriteLine("");
                            WriteLine("-----------------------------------------------------------------------------------");
                            WriteLine("-      Sorry Member does not exist. Please try again with different member.       -");
                            WriteLine("-----------------------------------------------------------------------------------");
                            WriteLine("");
                            invalidUserInput = false;
                            AddFirstName();
                        }
                        else
                        {
                            int deletionSuccessful = memberCollection.DeleteMember(indexNumberOfElementMatchingWithInput);//checks if member has borrowed any movie. so cannot remove member
                            if (deletionSuccessful == -1)
                            {
                                WriteLine("");
                                WriteLine("---------------------------------------------------------------------------------------------");
                                WriteLine("-      Sorry Member has borrowed some movies. Please try again with different member.       -");
                                WriteLine("-      Please Contact the member to return all the movies before removing the member        -");
                                WriteLine("-                                             Try Again?                                    -");
                                WriteLine("---------------------------------------------------------------------------------------------");
                                WriteLine("");
                                invalidUserInput = false;
                                AddFirstName();
                            }
                            else
                            {
                                WriteLine("");
                                WriteLine("-----------------------------------------------------------------------------------");
                                WriteLine("-              Congratualations! Member Successfully Deleted                      -");
                                WriteLine("-                      Want to remove more members?                               -");
                                WriteLine("-----------------------------------------------------------------------------------");
                                WriteLine("");
                                RemoveMember();
                                invalidUserInput = false;
                            }
                        }
                    }
                    else
                    {
                        WriteLine("");
                        WriteLine("Something wrong with input. Check if there are any spaces or special characters!");
                        Write("Last Name (without any space or special characters) or 0 to go back: ");
                        userInputLastName = ReadLine();
                    }
                }
            }
        }

        void FindContactNumber()//with the full name searches if the member exist
        {
            string userInputFirstName = "";
            string userInputLastName = "";
            string fullName = "";
            memberCollection.PrintMemberList();
            WriteLine("");
            WriteLine("---------------------------------------------------------------------------------------------");
            WriteLine("-                             Search Phone Number of Member                                 -");
            WriteLine("---------------------------------------------------------------------------------------------");
            WriteLine("Please enter all the details that is asked and do not leave the fields blank. Or enter 0 to go back");
            WriteLine("");
            AddFirstName();

            static bool NameValidation(string input)//validation to ensure no spaces or special characters exist in user input for first name and last name.
            {
                // Regular expression pattern to match only letters
                string pattern = @"^[a-zA-Z]+$";
                return Regex.IsMatch(input, pattern);
            }

            //takes first name input
            void AddFirstName()
            {
                bool invalidUserInput = true;
                WriteLine("");
                Write("First Name (without any space or special characters) or 0 to go back: ");
                userInputFirstName = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputFirstName == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputFirstName))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank please try again!");
                        Write("First Name (without any space or special characters) or 0 to go back: ");
                        userInputFirstName = ReadLine();
                    }
                    else if (NameValidation(userInputFirstName))
                    {
                        AddLastName();
                        invalidUserInput = false;
                    }
                    else
                    {
                        WriteLine("");
                        WriteLine("Something wrong with input. Check if there are any spaces or special characters!");
                        Write("First Name (without any space or special characters) or 0 to go back: ");
                        userInputFirstName = ReadLine();
                    }
                }
            }

            void AddLastName()//searches if the member exist and show the phone number
            {
                bool invalidUserInput = true;
                WriteLine(" ");
                Write("Last Name (without any space or special characters) or 0 to go back: ");
                userInputLastName = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputLastName == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputLastName))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank. Try again.");
                        Write("Last Name (without any space or special characters) or 0 to go back: ");
                        userInputLastName = ReadLine();
                    }
                    else if (NameValidation(userInputLastName))
                    {
                        fullName = userInputFirstName.ToUpper() + " " + userInputLastName.ToUpper();
                        int indexNumberOfElementMatchingWithInput = memberCollection.SearchIfMemberAlreadyExist(fullName);//search
                        if (indexNumberOfElementMatchingWithInput == -1)
                        {
                            WriteLine("");
                            WriteLine("-----------------------------------------------------------------------------------");
                            WriteLine("-      Sorry Member does not exist. Please try again with different member.       -");
                            WriteLine("-----------------------------------------------------------------------------------");
                            WriteLine("");
                            invalidUserInput = false;
                            AddFirstName();
                        }
                        else
                        {
                            Member member = memberCollection.ArrayToHoldMemberObject[indexNumberOfElementMatchingWithInput];
                            WriteLine("");
                            WriteLine("-----------------------------------------------------------------------------------");
                            WriteLine("-                        Found the member details!!                               -");
                            WriteLine("-----------------------------------------------------------------------------------");
                            WriteLine("");
                            WriteLine("||| Member First Name: "+member.FirstName+" | Member Last Name: "+member.LastName+" Member Full Name: "+member.FullName+" | Member Contact Number: "+member.PhoneNumber);
                            WriteLine("");
                            WriteLine("-----------------------------------------------------------------------------------");
                            WriteLine("-                Do you want to search for another member?                        -");
                            WriteLine("-----------------------------------------------------------------------------------");
                            invalidUserInput = false;
                            AddFirstName();
                        }
                    }
                    else
                    {
                        WriteLine("");
                        WriteLine("Something wrong with input. Check if there are any spaces or special characters!");
                        Write("Last Name (without any space or special characters) or 0 to go back: ");
                        userInputLastName = ReadLine();
                    }
                }
            }
        }

        void MembersBorrowingCertainMovie()
        {
            string userInputTitle = "";
            WriteLine("");
            WriteLine("----------------------------------------------------------------------------------------------------------");
            WriteLine("-                          Search which members are borrowing a specific movie                           -");
            WriteLine("----------------------------------------------------------------------------------------------------------");
            WriteLine("Please enter all the details that is asked and do not leave the fields blank. Or enter 0 to go back");
            WriteLine("");
            AddTitle();


            //this function takes title as input from user. And checks if the movie is already in MovieClassification hashtable
            //if the movie exists stores the movie object in a movie type variable and it prints the array that stores members name who borrowed the certain movie
            void AddTitle()//
            {

                bool invalidUserInput = true;
                WriteLine("");
                Write("Movie Title (case insensitive) or 0 to go back: ");
                userInputTitle = ReadLine();
                while (invalidUserInput == true)
                {
                    if (userInputTitle == "0")
                    {
                        DisplayMenuForStaff();
                        invalidUserInput = false;
                    }
                    else if (String.IsNullOrWhiteSpace(userInputTitle))
                    {
                        WriteLine("");
                        WriteLine("Input cannot be blank please try again!");
                        Write("Movie Title (case insensitive) or 0 to go back: ");
                        userInputTitle = ReadLine();
                    }
                    else
                    {
                        int indexOfBucketContainingMovieTitleThatUserInputed = movieCollection.SearchUsingTitleInputToDisplatInformation(userInputTitle.ToUpper());
                        if (indexOfBucketContainingMovieTitleThatUserInputed == -1)
                        {
                            WriteLine("");
                            WriteLine("--------------------------------------------------------------------------------------------------");
                            WriteLine("-                                  The Movie is not in the system.                               -");
                            WriteLine("-                                  Try again with different name                                 -");
                            WriteLine("--------------------------------------------------------------------------------------------------");
                            WriteLine("");
                            Write("Movie Title (case insensitive) or 0 to go back: ");
                            userInputTitle = ReadLine();
                        }
                        else
                        {
                            Movie movieObjectSearchedByUser = movieCollection.Table[indexOfBucketContainingMovieTitleThatUserInputed];
                            WriteLine("");
                            WriteLine("--------------------------------------------------------------------------------------------------");
                            WriteLine("-                                       The Movie is found!!                                     -");
                            WriteLine("--------------------------------------------------------------------------------------------------");
                            WriteLine("");
                            movieObjectSearchedByUser.PrintMembersNameBorrowingMovie(userInputTitle.ToUpper());
                            WriteLine("");
                            WriteLine("--------------------------------------------------------------------------------------------------");
                            WriteLine("                                   Want to search another movie?                                 -");
                            WriteLine("--------------------------------------------------------------------------------------------------");
                            AddTitle();
                            invalidUserInput = false;
                        }
                    }
                }
            }
        }

    }
}