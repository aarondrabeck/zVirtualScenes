/*
usage:
require("settings.js");
var g = new gmail(settings);

or

settings = {
	Email : "MY.EMAIL.ADDRESS@gmail.com",
	EmailPassword : "MY PASSWORD",
	Name : "MY NAME"
}
var g = new gmail(settings);
g.Send("TO.ADDRESS@foo.com", "TO NAME", "Hi There", "This is the body\n\nBye.");


*/

gmail = function(config) {
	this.config = config;
	this.fromAddress = new System.Net.Mail.MailAddress(config.Email, config.Name);
	this.fromPassword = config.EmailPassword;
		
	this.Send = function(address, name, subject, body) {
		var toAddress = new System.Net.Mail.MailAddress(address, name);
		
		var smtp = new System.Net.Mail.SmtpClient();
		smtp.Host = "smtp.gmail.com";
		smtp.Port = 587;
		smtp.EnableSsl = true;
		smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
		smtp.UseDefaultCredentials = false;
		smtp.Credentials = new System.Net.NetworkCredential(this.fromAddress.Address, this.fromPassword);			 
		var message = new System.Net.Mail.MailMessage(this.fromAddress, toAddress);
		message.Subject = subject;
		message.Body = body;
		smtp.Send(message);
	}
}