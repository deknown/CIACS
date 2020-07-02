from django.db import models


# Create your models here.
class Structure(models.Model):
    id = models.AutoField(primary_key=True)
    parent_id = models.ForeignKey('self', on_delete=models.CASCADE)
    name = models.CharField(max_length=50)

    def __str__(self):
        return self.name

class Computers(models.Model):
    id = models.AutoField
    computer_id = models.IntegerField(null=False)
    ip = models.CharField(max_length=50)

    class Meta:
        unique_together = (('id', 'computer_id'),)


class Os(models.Model):
    id = models.AutoField
    name = models.CharField(max_length=50, blank=True, null=True)
    build = models.CharField(max_length=50, blank=True, null=True)
    bits = models.IntegerField(max_length=8, blank=True, null=True)
    computer_id = models.ForeignKey(Computers, default=None, on_delete=models.CASCADE)

    class Meta:
        unique_together = (('id', 'computer_id'),)


class Motherboards(models.Model):
    id = models.AutoField
    computer_id = models.ForeignKey(Computers, on_delete=models.CASCADE)
    manufacturer = models.CharField(max_length=250, blank=True, null=True)
    name = models.CharField(max_length=50, blank=True, null=True)

    class Meta:
        unique_together = (('id', 'computer_id'),)


class Cpus(models.Model):
    id = models.AutoField
    computer_id = models.ForeignKey(Computers, on_delete=models.CASCADE)
    manufacturer = models.CharField(max_length=50, blank=True, null=True)
    name = models.CharField(max_length=50, blank=True, null=True)
    cores = models.IntegerField(max_length=4, blank=True, null=True)
    threads = models.IntegerField(max_length=5, blank=True, null=True)
    speed = models.IntegerField(max_length=5, blank=True, null=True)
    bits = models.IntegerField(max_length=8, blank=True, null=True)

    class Meta:
        unique_together = (('id', 'computer_id'),)


class Gpus(models.Model):
    id = models.AutoField
    computer_id = models.ForeignKey(Computers, on_delete=models.CASCADE)
    manufacturer = models.CharField(max_length=50, blank=True, null=True)
    name = models.CharField(max_length=50, blank=True, null=True)
    capacity = models.IntegerField(max_length=15, blank=True, null=True)

    class Meta:
        unique_together = (('id', 'computer_id'),)


class Rams(models.Model):
    id = models.AutoField
    computer_id = models.ForeignKey(Computers, on_delete=models.CASCADE)
    manufacturer = models.CharField(max_length=50, blank=True, null=True)
    name = models.CharField(max_length=50, blank=True, null=True)
    type = models.CharField(max_length=50, blank=True, null=True)
    capacity = models.IntegerField(max_length=15, blank=True, null=True)

    class Meta:
        unique_together = (('id', 'computer_id'),)


class Disks(models.Model):
    id = models.AutoField
    computer_id = models.ForeignKey(Computers, on_delete=models.CASCADE)
    type = models.CharField(max_length=10, blank=True, null=True)
    capacity = models.IntegerField(max_length=50, blank=True, null=True)

    class Meta:
        unique_together = (('id', 'computer_id'),)


class Software(models.Model):
    id = models.AutoField
    name = models.CharField(max_length=250, blank=True, null=True)
    version = models.CharField(max_length=250, blank=True, null=True)
    computer_id = models.ForeignKey(Computers, default=None, on_delete=models.CASCADE)

    class Meta:
        unique_together = (('id', 'computer_id'),)


