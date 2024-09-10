Remove-Item * -Include *.pfx
dotnet dev-certs https -v -ep certificate.pfx -p fdd45d7b-f9f6-431b-be3b-9e6d90a28e7d --trust
cd..