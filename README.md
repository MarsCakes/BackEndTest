[SQLQuery2.sql](https://github.com/user-attachments/files/24085514/SQLQuery2.sql)

CREATE SEQUENCE EmpSeq
    START WITH 1
    INCREMENT BY 1;

CREATE TABLE Employee (
    NIK VARCHAR(10) NOT NULL PRIMARY KEY
        DEFAULT ('A00' + RIGHT(''+CAST(NEXT VALUE FOR EmpSeq AS VARCHAR(3)), 3)),
    Nama VARCHAR(100) NOT NULL
);


INSERT INTO Employee(Nama) VALUES 
('Andi'),
('Susi'),
('Toni'),
('Hendra');

CREATE TABLE absen (
    NIK VARCHAR(10),
    TanggalAbsen DATE, 
    PRIMARY KEY(NIK,TanggalAbsen),
    FOREIGN KEY (NIK) REFERENCES Employee(NIK)
);
