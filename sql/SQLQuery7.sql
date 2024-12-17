create table Comments(
commentID int identity(1,1) primary key,
message text not null,
masterID int,
requestID int,
foreign key (masterID) references Users(userID),
foreign key (requestID) references Requests(requestsID)
);
insert into Comments (message, masterID, requestID) values
('����� �������.',2,1),
('����� �����������!',3,2),
('����� �����������!',3,3);