<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DynamicImageHandler.Web._Default" %>

<%@ Import Namespace="System.Web.Hosting" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.5/css/bootstrap.css" />

    <link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/bootstrap-material-design/0.3.0/css/material.css" />
    <link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/bootstrap-material-design/0.3.0/css/ripples.css" />
    <link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/bootstrap-material-design/0.3.0/css/roboto.css" />
    <link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/bootstrap-material-design/0.3.0/css/material-fullpalette.css" />


    <script type="text/javascript" src="//cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.5/js/bootstrap.js"></script>
    <script type="text/javascript" src="//cdnjs.cloudflare.com/ajax/libs/bootstrap-material-design/0.3.0/js/material.js"></script>
    <script type="text/javascript" src="//cdnjs.cloudflare.com/ajax/libs/bootstrap-material-design/0.3.0/js/ripples.js"></script>

    <base href="<%=HostingEnvironment.ApplicationVirtualPath%>" />
    
    <style type="text/css">
        img {
            border: 1px solid #eee;
            -webkit-transition: all .25s ease;
            -moz-transition: all .25s ease;
            -ms-transition: all .25s ease;
            -o-transition: all .25s ease;
            transition: all .25s ease;
        }
        img:hover {
            border: 1px solid #999;
            -webkit-transform:scale(1.25);
            -moz-transform:scale(1.25);
            -ms-transform:scale(1.25);
            -o-transform:scale(1.25);
            transform:scale(1.25);
        }
    </style>

    <title>Dynamic Image Handler Demo</title>
</head>
<body>
    <form id="form1" runat="server">

    <nav class="navbar navbar-default">
      <div class="container-fluid">
        <div class="navbar-header">
          <a class="navbar-brand" href="#">Dynamic Image Handler Demo</a>
        </div>
      </div>
    </nav>

        <div class="container pages">

            <div class="well page">
                <h2 class="header">Preserve Aspect Ratio</h2>
                <hr />

                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=300" />
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=200" />
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=100" />
            </div>

            <div class="well page">
                <h2 class="header">No Resize Constraint</h2>
                <hr />
                
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=300&width=50&resize_constrain=false" />
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=200&width=100&resize_constrain=false" />
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=100&width=200&resize_constrain=false" />
            </div>

            <div class="well page">
                <h2 class="header">Force Square Resize (JPEG to PNG Conversion)</h2>
                <hr />
                
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&format=png&width=400&height=400&resize_square=true" />
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&format=png&width=250&height=250&resize_square=true" />
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&format=png&width=250&height=250&resize_square=true&resize_bgcolor=transparent" />
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&format=png&width=50&height=50&resize_square=true" />
            </div>

            <div class="well page">
                <h2 class="header">Rotated and Resized</h2>
                <hr />
                45° -
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=100&width=200&rotate=45&format=png" alt="" />

                90° -
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=100&width=200&rotate=90" alt="" />

                135° -
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=100&width=200&rotate=135&format=png" alt="" />

                180° -
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=100&width=200&rotate=180" alt="" />
            </div>
            
            <div class="well page">
                <h2 class="header">Greyscale</h2>
                <hr />
                
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&height=300&grayscale=true" />
            </div>
            
            <div class="well page">
                <h2 class="header">Zoom</h2>
                <hr />
                
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&zoom=0.5" />
            </div>
            
            <div class="well page">
                <h2 class="header">Watermark</h2>
                <hr />
                
                <img src="ImageHandler.ashx?src=/App_Data/Frangipani Flowers.jpg&width=300&watermark=Hello%20World&watermark_opacity=15" />
            </div>
        </div>
    </form>
</body>
</html>
