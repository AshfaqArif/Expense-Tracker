# tracker/forms.py
from django import forms
from django.contrib.auth.forms import UserCreationForm
from django.utils import timezone  # Add this import
from .models import CustomUser, Expense

class CustomUserCreationForm(UserCreationForm):
    email = forms.EmailField(required=True)
    phone_number = forms.CharField(required=True)
    address = forms.CharField(widget=forms.Textarea, required=True)

    class Meta:
        model = CustomUser
        fields = ('username', 'email', 'phone_number', 'address', 'password1', 'password2')

class ExpenseForm(forms.ModelForm):
    class Meta:
        model = Expense
        fields = ['date', 'item_name', 'cost']
    
    date = forms.DateField(
        widget=forms.DateInput(attrs={'type': 'date'}),
        initial=timezone.now().date()
    )

    def clean_date(self):
        date = self.cleaned_data['date']
        if date > timezone.now().date():
            raise forms.ValidationError("You cannot add expenses for future dates.")
        return date
    

from django import forms
from .models import Contact

class ContactForm(forms.ModelForm):
    class Meta:
        model = Contact
        fields = ['name', 'email', 'message']
        
        widgets = {
            'name': forms.TextInput(attrs={'placeholder': 'Your Name'}),
            'email': forms.EmailInput(attrs={'placeholder': 'Your Email'}),
            'message': forms.Textarea(attrs={'placeholder': 'Your Message', 'rows': 4}),
        }

    def clean_name(self):
        name = self.cleaned_data.get('name')
        if not name.isalpha():
            raise forms.ValidationError('Name must contain only alphabets.')
        if len(name) < 3:
            raise forms.ValidationError('Name must be at least 3 characters long.')
        return name

    def clean_email(self):
        email = self.cleaned_data.get('email')
        if not email:
            raise forms.ValidationError('Email is required.')
        if Contact.objects.filter(email=email).exists():
            raise forms.ValidationError('This email address is already in use.')
        return email