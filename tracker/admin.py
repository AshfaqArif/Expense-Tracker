# admin.py
from django.contrib import admin
from .models import Expense , Contact

@admin.register(Expense)
class ExpenseAdmin(admin.ModelAdmin):
    list_display = ('date', 'item_name', 'cost')  # Customize this as needed


from django.contrib import admin
from .models import Contact

admin.site.register(Contact)

