<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DynamicImageHandler.Web._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		Preserve aspect ratio:<br/>
		<asp:Image runat="server" ID="img1" ImageUrl="~/ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=300" />
		<asp:Image runat="server" ID="img2" ImageUrl="~/ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=200" alt="" />
		<asp:Image runat="server" ID="img3" ImageUrl="~/ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=100" alt="" />

		<br/><br/>
		Wierd resizes<br/>
		<asp:Image runat="server" ID="img4" ImageUrl="~/ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=300&width=50" alt="" />
		<asp:Image runat="server" ID="img5" ImageUrl="~/ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=200&width=100" alt="" />
		<asp:Image runat="server" ID="img6" ImageUrl="~/ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=100&width=200" alt="" />
		
		<br/><br/>
		Rotated and resized<br/>
		45° - <asp:Image runat="server" ID="img7" ImageUrl="~/ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=100&width=200&rotate=45&format=png" alt="" />
		90° - <asp:Image runat="server" ID="img8" ImageUrl="~/ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=100&width=200&rotate=90" alt="" /><br />
		135° - <asp:Image runat="server" ID="img9" ImageUrl="~/ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=100&width=200&rotate=135&format=png" alt="" />
		180° - <asp:Image runat="server" ID="img10" ImageUrl="~/ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=100&width=200&rotate=180" alt="" /><br />
    </div>
    </form>
</body>
</html>
