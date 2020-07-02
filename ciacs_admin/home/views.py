from django.shortcuts import render
from django.http import HttpResponseRedirect
from django.contrib.auth.decorators import login_required


# Create your views here.
@login_required
def index(request):
	return HttpResponseRedirect("<h1>Home page</h1>")