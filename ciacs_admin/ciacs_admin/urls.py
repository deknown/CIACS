from django.contrib import admin
from django.urls import path, include

urlpatterns = [
    path('', admin.site.urls),
    path('appeal/', include('appeals.urls')),
    path('report/', include('reports.urls')),
]
