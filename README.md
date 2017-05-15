# Excel2Xsd
xsd builder from excel

## Usage:
### 1. An excel file struct like this

|Name|Name2|Name3|Type|Metadata|Range|Remark|
|:--|:--|:--|:--|:--|:--|:--|
|ComplexField1|||ComplexType1||||
||IntField1|||int||some remark|
||EnumField1||FlightTripType|Enum|1=One=enumremark1<br>2=Two=enumremark2<br>3=Three=enumremark3|some remark|
||ComplexListField2||ComplexType2|List||this is a list of ComplexType2 object|
|||StringField1||string||this is a string|
|||StringField2||string||this is also a string|

### 2. Drag the excel file onto the Excel2Xsd.exe(after build)
### 3. The program will generate a temp.xml in the active folder, that's the xsd out put

## Note:
#### Metadata is some basic xsd element type
