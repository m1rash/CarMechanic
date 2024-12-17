create table Requests (
requestsID int identity(1,1) primary key,
startDate date not null,
carID int,
problemDescription text not null,
statusID int,
complectionDate date,
masterID int,
clientID int,
foreign key (carID) references Cars(carID),
foreign key (statusID) references Statuses(statusID),
foreign key (masterID) references Users(userID),
foreign key (clientID) references Users(userID)
);
insert into Requests (startDate, carID, problemDescription, statusID, complectionDate, masterID, clientID) values
('2024-09-09',1,'Отказали тормоза.',2,NULL,2,6),
('2024-10-10',2,'Замена масла.',2,NULL,3,7),
('2024-11-11',3,'В салоне запах бензина.',3,'2024-11-12',3,8),
('2024-10-10',4,'Проблемно крутится руль',1,NULL,NULL,7),
('2024-11-11',5,'Проблемно крутится руль',1,NULL,NULL,8);