# Version_2.1
#NSB Sample with Nhibernate Persistence and SQLTransport

The objective of the sample application is to illustrate how to use NHibernate Persistance and SQLTransport in NServiceBus

    Technologies used/required in this sample:
    - Entity Framework 6
    - NServiceBus 5.0
    - MSSQL
	-NServiceBus.Nhibernate	   
    
Code Walkthrough

This sample contains three projects

    Shared - A class library containing common code including the messages definitions.
    Sender - A console application responsible for sending the initial OrderSubmitted message and processing the follow-up OrderAccepted message.
    Receiver - A console application responsible for processing the order message.
	
Sender project
The Sender does not store any data. 
It mimics the front-end system where orders are submitted by the end users and passed via the bus to the back-end.
It is configured to use SQLServer transport with NHibernate persistence. 
It publishes the OrderSubmitted message to receiver endpoint.The connection strings for both persistence and transport are stored into app.config file.
Change the connection string according to your database.
    
Receiver project
The Receiver mimics a back-end system. It is also configured to use SQL Server transport with NHibernate persistence but instead of
hard-coding the other endpoint's schema, it uses a convention based on the endpoint's queue name(defined using UseSpecificConnectionInformation).
OrderSubmittedHandler handles the order and sends OrderAcceptd reply to sender over bus.

Follow below Steps to Execute /configure on local system
1:Download the solution 
2:Resolve the dependencies and try to build it.
3:Open the MSSQL server and create a database with name 'Shared'.
4:Change the connection string in app.config of both receiver and sender project.
5:First run/debug the receiver project , it will create SQLTransport tables(audit,error,subscription...etc) and receiver tables(Receiver,Retries,timeouts...etc) also.   
6:Verify the tables created in step 5
7:Run/Debug the Sender project. It will create a sender tables and will publish a order to Receiver but as receiver is not running this order will be saved into the database configured.(SQLTransport).
8:Verify the transport tables(select * from audit). 
9:Run/debug the receiver so that the order can be processed. Verify the receiver tables data.
 
Pending coverage of:
Use of different schemas for sender and receiver within one database.
Sagas
 
